using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    private void Awake()
    {
        SetCanvasActive(canvasNewUser, false);
    }

    public async void TryLogin()
    {
        string mailString = mail.text;
        string passwordString = password.text;

        if (mailString != "" && passwordString != "")
        {
            SetCanvasActive(canvasBase, false);
            loadingSpinner.SetActive(true);

            bool loginSuccess = await LoginAsync(mailString, passwordString);

            if (loginSuccess)
            {
                try
                {
                    await ServerConnection.Instance.GameInfoInitAsync();
                    await ServerConnection.Instance.UserInfoInitAsync();

                    RequestUserInfo.Instance.GetUserData(PlayerPrefs.GetString("UserID"), userdata =>
                    {


                        if (userdata.currentCritteron == "")
                        {
                            RequestGameInfo.Instance.GetCritteronByID("677123cbf8e9b02d66239c82", critteron =>
                            {
                                RequestUserInfo.Instance.ModifyUserCritteron(PlayerPrefs.GetString("UserID"), "677123cbf8e9b02d66239c82", currentLife: critteron.life, level: 1);
                                RequestUserInfo.Instance.ModifyUserData(PlayerPrefs.GetString("UserID"), currentCritteron: "677123cbf8e9b02d66239c82", level: 1);
                                RequestUserInfo.Instance.ModifyUserRooms(PlayerPrefs.GetString("UserID"), "6755c9dab8d0a120196ac902");

                                StartCoroutine("changeScene");
                            });

                        }
                        else
                        {
                            RequestUserInfo.Instance.GetUserSentFriend(PlayerPrefs.GetString("UserID"), sentList =>
                            {
                                RequestUserInfo.Instance.GetUserPendingFriend(PlayerPrefs.GetString("UserID"), pendingList =>
                                {

                                    if (sentList.Count != 0 && pendingList.Count != 0)
                                    {
                                        foreach (var item in sentList)
                                        {
                                            foreach (var item2 in pendingList)
                                            {
                                                if (item.friendID == item2.friendID)
                                                {
                                                    RequestUserInfo.Instance.ModifySocialStat(PlayerPrefs.GetString("UserID"), item.friendID);
                                                    RequestUserInfo.Instance.RemovePendingFriend(PlayerPrefs.GetString("UserID"), item.friendID);
                                                    RequestUserInfo.Instance.RemoveSentFriend(PlayerPrefs.GetString("UserID"), item.friendID);
                                                }
                                            }
                                        }


                                        loadingSpinner.SetActive(false);
                                        SceneManager.LoadScene("Hotel");
                                    }
                                    else
                                    {
                                        RequestUserInfo.Instance.ModifyUserCritteronLifeTime(PlayerPrefs.GetString("UserID"));
                                        loadingSpinner.SetActive(false);
                                        SceneManager.LoadScene("Hotel");
                                    }
                                });
                            });


                        }
                    });


                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error during initialization: {ex.Message}");
                    SetCanvasActive(canvasBase, true);
                }
            }
            else
            {
                SetCanvasActive(canvasBase, true);
            }

            loadingSpinner.SetActive(false);
        }
    }

    private IEnumerator changeScene()
    {
        yield return new WaitForSeconds(4);
        loadingSpinner.SetActive(false);
        SceneManager.LoadScene("Hotel");
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

 

    public void Test()
    {
        RequestUserInfo.Instance.GetUserByID("673cc422cd24a40417560f59", (asuidas) =>
        {
            Debug.Log(asuidas);
        }
        );
    }
}