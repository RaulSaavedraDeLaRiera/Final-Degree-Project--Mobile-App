using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSocial : MonoBehaviour
{
    [SerializeField]
    GameObject critteronInfo;

    string idCritteron;

    public void SetidCritteron(string id)
    {
        idCritteron = id;
    }

    public void SetCurrentCritteron()
    {
        RequestUserInfo.Instance.ModifyUserData(PlayerPrefs.GetString("UserID"), currentCritteron: idCritteron);

        if(critteronInfo != null)
        {
            Transform healthC = critteronInfo.transform.Find("Health");
            Transform levelC = critteronInfo.transform.Find("Level");
            TextMeshProUGUI nameText = healthC.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI levelCText = levelC.GetComponentInChildren<TextMeshProUGUI>();

            RequestUserInfo.Instance.GetUserData(PlayerPrefs.GetString("UserID"), userData =>
            {
                RequestUserInfo.Instance.GetUserCritteronsByID(PlayerPrefs.GetString("UserID"), userData.currentCritteron.ToString(), critteron =>
                {

                    levelCText.text = critteron.level.ToString();
                });
                RequestGameInfo.Instance.GetCritteronByID(userData.currentCritteron.ToString(), critteron2 =>
                {
                    nameText.text = critteron2.name;
                    SceneManager.LoadScene("UserInfo");
                });
            });
        }
    }
}
