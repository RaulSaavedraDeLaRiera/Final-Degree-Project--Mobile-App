using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static I_User;


public class Login : MonoBehaviour
{
    [SerializeField]
    TMP_InputField mail, password, mailC, passwordC, nameC;
    [SerializeField]
    private GameObject loadingSpinner;
    [SerializeField]
    private Canvas canvasBase;
    [SerializeField]
    private Canvas canvasNewUser;

    const string initScene = "Principal";

    private void Awake()
    {
        SetCanvasActive(canvasNewUser, false);
    }

    void Start()
    {
        AlreadyLog();
        AudioManager.m.PlaySound("upgrade");
    }

    public async void LoginGame(string mailString, string passwordString)
    {
        if (mailString != "" && passwordString != "")
        {
            SetCanvasActive(canvasBase, false);
            loadingSpinner.SetActive(true);

            bool loginSuccess = await LoginAsync(mailString, passwordString);

            if (loginSuccess)
            {
                Debug.Log("Hciendo login");
                SaveLoginData(mailString, passwordString);
                try
                {
                    await ServerConnection.Instance.GameInfoInitAsync();
                    await ServerConnection.Instance.UserInfoInitAsync();

                    var user = await RequestUserInfo.Instance.GetUserAsync(PlayerPrefs.GetString("UserID"));
                    Debug.Log("Obtengo user");

                    if (user.userData.currentCritteron == "")
                    {
                        var critteron = await RequestGameInfo.Instance.GetCritteronByIDAsync("67cdbaeb9efa2340c96eecc4");
                        PlayerPrefs.SetString("CurrentCritteronID", "67cdbaeb9efa2340c96eecc4");
                        PlayerPrefs.Save();
                        RequestUserInfo.Instance.ModifyUserCritteron(PlayerPrefs.GetString("UserID"), "67cdbaeb9efa2340c96eecc4", currentLife: critteron.life, level: 1);
                        RequestUserInfo.Instance.ModifyUserData(PlayerPrefs.GetString("UserID"), currentCritteron: "67cdbaeb9efa2340c96eecc4", level: 1, money: 100);
                        RequestUserInfoSocial.Instance.ModifyPersonalStats(PlayerPrefs.GetString("UserID"), daysStreak: 1);
                        RequestUserInfo.Instance.ModifyUserRooms(PlayerPrefs.GetString("UserID"), "6755c9dab8d0a120196ac902");

                        XasuControl.MessageWithCustomVerb(
                            actionId: "FIRST_LOGIN",
                            verbId: "http://adlnet.gov/expapi/verbs/initialized",
                            verbDisplay: "initialized",
                            timestamp: DateTime.UtcNow
                        );
                        StartCoroutine("changeScene");
                    }
                    else
                    {

                        Debug.Log("Modificacndo datos");

                        PlayerPrefs.SetString("CurrentCritteronID", user.userData.currentCritteron);
                        PlayerPrefs.SetInt("FriendTogetherCombat", 0);
                        PlayerPrefs.Save();

                        var a = user.userData.lastClosedTime;

                        DateTimeOffset lastSeenOffset = DateTimeOffset.FromUnixTimeSeconds(user.userData.lastClosedTime / 1000);
                        DateTime lastDate = lastSeenOffset.Date;
                        DateTime currentDate = DateTime.UtcNow.Date;

                        TimeSpan difference = currentDate - lastDate;

                        if (difference.Days == 1)
                            RequestUserInfoSocial.Instance.ModifyPersonalStats(PlayerPrefs.GetString("UserID"), daysStreak: user.personalStats.daysStreak + 1);
                        else if (difference.Days > 1)
                            RequestUserInfoSocial.Instance.ModifyPersonalStats(PlayerPrefs.GetString("UserID"), daysStreak: 1);

                        RequestUserInfo.Instance.ModifyUserCritteronLifeTime(PlayerPrefs.GetString("UserID"));

                        XasuControl.MessageWithCustomVerb(
                            actionId: "LOGIN",
                            verbId: "http://adlnet.gov/expapi/verbs/initialized",
                            verbDisplay: "initialized",
                            timestamp: DateTime.UtcNow
                        );

                        loadingSpinner.SetActive(false);
                        Debug.Log("Cambiando escena");

                        SceneManager.LoadScene(initScene);
                    }

                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error during initialization: {ex.Message}");
                    SetCanvasActive(canvasBase, true);
                }
            }
            else
            {
                PlayerPrefs.DeleteKey("email");
                PlayerPrefs.DeleteKey("password");
                PlayerPrefs.Save();

                SetCanvasActive(canvasBase, true);
            }

            AudioManager.m.PlaySound("changescene");
            loadingSpinner.SetActive(false);
        }
    }

    public async void TryLogin()
    {
        string mailString = mail.text;
        string passwordString = password.text;

        LoginGame(mailString, passwordString);
    }

    void SaveLoginData(string email, string password)
    {
        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("password", password);
        PlayerPrefs.Save();

    }

    private IEnumerator changeScene()
    {
        yield return new WaitForSeconds(4);
        loadingSpinner.SetActive(false);
        SceneManager.LoadScene(initScene);
    }

    private Task<bool> LoginAsync(string mail, string password)
    {
        var tcs = new TaskCompletionSource<bool>();

        RequestUserInfo.Instance.Login(mail, password, (success) =>
        {
            tcs.SetResult(success);
        });

        return tcs.Task;
    }


    private void SetCanvasActive(Canvas canvas, bool active)
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(active);
        }
    }

    public void ChangeCanvas()
    {
        SetCanvasActive(canvasBase, !canvasBase.gameObject.activeSelf);
        SetCanvasActive(canvasNewUser, !canvasNewUser.gameObject.activeSelf);
    }

    public void CreateNewUser()
    {
        string nameString = nameC.text;
        string passwordString = passwordC.text;
        string mailString = mailC.text;

        if (nameString != "" && passwordString != "" && mailString != "")
        {
            loadingSpinner.SetActive(true);

            if (nameString != "" && passwordString != "" && mailString != "")
            {
                StartCoroutine(ServerConnection.Instance.CreateNewUser(nameString, passwordString, mailString, (resolution) =>
                {
                    loadingSpinner.SetActive(false);
                    if (resolution)
                    {
                        Debug.Log("User create");
                    }
                    else
                    {
                        Debug.Log("Server error: Unable to create user");

                    }
                }));
            }

            loadingSpinner.SetActive(false);
        }
        ChangeCanvas();

    }


    async void AlreadyLog()
    {
        if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey("password"))
        {
            string email = PlayerPrefs.GetString("email");
            string password = PlayerPrefs.GetString("password");

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                Debug.Log("El usuario ya ha iniciado sesión.");
                AudioManager.m.PlaySound("changescene");
                LoginGame(email, password);
            }
        }

    }



    public void Test()
    {
        RequestUserInfo.Instance.GetUserByID("673cc422cd24a40417560f59", (asuidas) =>
        {
            Debug.Log(asuidas);
        }
        );
    }
}