using System;
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
            loadingSpinner.SetActive(false);

            if (loginSuccess)
            {
                try
                {
                    await ServerConnection.Instance.GameInfoInitAsync();
                    await ServerConnection.Instance.UserInfoInitAsync();

                    SceneManager.LoadScene("Hotel");
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
        }
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
