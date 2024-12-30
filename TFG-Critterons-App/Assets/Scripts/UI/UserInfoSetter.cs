using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInfo : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;

    [SerializeField]
    GameObject userInfo;

    [SerializeField]
    GameObject currentCritteron;

    [SerializeField]
    GameObject critteron;

    [SerializeField]
    GameObject critteronsRoot;

    void Start()
    {
        Transform level = userInfo.transform.Find("level");
        Transform name = userInfo.transform.Find("NameText");
        TextMeshProUGUI nameText = name.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI levelText = level.GetComponentInChildren<TextMeshProUGUI>();

        Transform healthC = currentCritteron.transform.Find("Health");
        Transform levelC = currentCritteron.transform.Find("Level");
        TextMeshProUGUI healthCText = healthC.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI levelCText = levelC.GetComponentInChildren<TextMeshProUGUI>();


        RequestUserInfo.Instance.GetUserData(PlayerPrefs.GetString("UserID"), userData =>
        {
            nameText.text = userData.name;
            levelText.text = userData.level.ToString();

            RequestUserInfo.Instance.GetUserCritteronsByID(PlayerPrefs.GetString("UserID"), userData.currentCritteron.ToString(), critteron =>
            {

                levelCText.text = critteron.level.ToString();
            });
            RequestGameInfo.Instance.GetCritteronByID(userData.currentCritteron.ToString(), critteron2 =>
            {
                healthCText.text = critteron2.name;
            });
        });

        RequestUserInfo.Instance.GetUserCritterons(PlayerPrefs.GetString("UserID"), critteronsList =>
        {
            foreach (var critteronInfo in critteronsList)
            {
                GameObject newCritteron = Instantiate(critteron, critteronsRoot.transform);

                ButtonSocial b = newCritteron.GetComponent<ButtonSocial>();
                b.SetidCritteron(critteronInfo.critteronID);

                Transform healthDataTransform = newCritteron.transform.Find("Health");
                TextMeshProUGUI healthText = healthDataTransform.GetComponentInChildren<TextMeshProUGUI>();
                healthText.text = critteronInfo.currentLife.ToString();

                RequestGameInfo.Instance.GetCritteronByID(critteronInfo.critteronID, critteron =>
                {
                    Transform data = newCritteron.transform.Find("Data");
                    TextMeshProUGUI dataText = data.GetComponentInChildren<TextMeshProUGUI>();
                    dataText.text = critteron.name;
                });

            }

        });
    }


    public void LoadHotel()
    {
        SceneManager.LoadScene("Hotel");
    }
}
