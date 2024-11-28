using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using static I_UserInfo;

public class Login : MonoBehaviour
{
    [SerializeField]
    TMP_InputField name, password;

    public void TryLogin()
    {
        string nameString, passwordString;

        

        nameString = name.text;
        passwordString = password.text;



        //solicitas el token al servidor
        StartCoroutine(ServerConnection.Instance.LoginToken(nameString, passwordString, (token) =>
        {
            if (token != "")
            {
                PlayerPrefs.SetString("token", token);
                PlayerPrefs.Save();
                Debug.Log(token);
            }
            else
            {
                Debug.Log("Server error");
            }
        }));
    }

    public void Test()
    {

        RequestUserInfo.Instance.GetUserByID("673cc422cd24a40417560f59", (asuidas) => {

            Debug.Log(asuidas);
        }

        );
    }
}
