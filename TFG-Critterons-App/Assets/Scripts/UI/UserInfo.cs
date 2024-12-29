using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
                healthCText.text = critteron.currentLife.ToString();
                levelCText.text = critteron.level.ToString();
            });
        });

        // Obtener todos los critterons y generarlos en CritteronsRoot
        RequestUserInfo.Instance.GetUserCritterons(PlayerPrefs.GetString("UserID"), critteronsList =>
        {
            foreach (var critteronInfo in critteronsList)
            {
                GameObject newCritteron = Instantiate(critteron, critteronsRoot.transform);
              
            }
        });
    }

}
