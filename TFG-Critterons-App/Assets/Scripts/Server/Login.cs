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
    private Canvas canvasBaseGroup;
    [SerializeField]
    private Canvas canvasNewUserGroup;

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

        RequestUserInfo.Instance.Login(mailString, passwordString, (success) =>
        {
            loadingSpinner.SetActive(false);
            if (success)
            {
                ServerConnection.Instance.GameInfoInit();
                ServerConnection.Instance.UserInfoInit();
                SceneManager.LoadScene("Hotel");
            }
            else
            {
                SetCanvasActive(canvasBaseGroup, true);
            }
        });
    }

    private void SetCanvasActive(Canvas canvas, bool active)
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(active);
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
                }
                else
                {
                    Debug.Log("Server error: Unable to create user");

                }
            }));
        }

        loadingSpinner.SetActive(false);
        SetCanvasActive(canvasNewUserGroup, false);
        SetCanvasActive(canvasBaseGroup, true);

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
