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
    private CanvasGroup canvasBaseGroup;
    [SerializeField]
    private CanvasGroup canvasNewUserGroup;

    private void Awake()
    {
        SetCanvasActive(canvasNewUserGroup, false);
    }

    public void TryLogin()
    {
        string mailString = mail.text;
        string passwordString = password.text;
        SetCanvasActive(canvasBaseGroup, false);
        loadingSpinner.SetActive(true);

        StartCoroutine(ServerConnection.Instance.LoginToken(mailString, passwordString, (token) =>
        {
            loadingSpinner.SetActive(false);

            if (token != "")
            {
                StartCoroutine(ServerConnection.Instance.GameInfoInit());
                StartCoroutine(ServerConnection.Instance.UserInfoInit());
                PlayerPrefs.SetString("token", token);
                PlayerPrefs.Save();
                Debug.Log($"Token received: {token}");
                SceneManager.LoadScene("Hotel");
            }
            else
            {
                Debug.Log("Server error: Unable to login");
                SetCanvasActive(canvasBaseGroup, true);
            }
        }));
    }

    private void SetCanvasActive(CanvasGroup canvas, bool active)
    {
        if (canvas != null)
        {
            canvas.interactable = active;
            canvas.blocksRaycasts = active;
            canvas.alpha = active ? 1 : 0;
        }
    }

    public void CreateNewUserChangeMenu()
    {
        SetCanvasActive(canvasBaseGroup, false);
        SetCanvasActive(canvasNewUserGroup, true);  
    }

    public void CreateNewUser()
    {
        string nameString = nameC.text;
        string passwordString = passwordC.text;
        string mailString = mailC.text;
        loadingSpinner.SetActive(true);
        if (nameString != "" && passwordString != "" && mailString != "")
        {
            StartCoroutine(ServerConnection.Instance.CreateNewUser(nameString, passwordString, mailString, (resolution) =>
            {
                loadingSpinner.SetActive(false);
                if (resolution)
                {
                    Debug.Log("User create");
                    loadingSpinner.SetActive(false);
                    SetCanvasActive(canvasNewUserGroup, false);
                    SetCanvasActive(canvasBaseGroup, true);
                }
                else
                {
                    Debug.Log("Server error: Unable to create user");
                    loadingSpinner.SetActive(false);
                    SetCanvasActive(canvasNewUserGroup, false);
                    SetCanvasActive(canvasBaseGroup, true);
                }
            }));
        }
        else
        {
            loadingSpinner.SetActive(false);
            SetCanvasActive (canvasNewUserGroup, false);
            SetCanvasActive(canvasBaseGroup, true);
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
