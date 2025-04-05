using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static I_GameInfo;
using static I_User;

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

    [SerializeField]
    GameObject generalInfo;

    void Start()
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
                                RequestUserInfoSocial.Instance.ModifySocialStat(PlayerPrefs.GetString("UserID"), item.friendID);
                                RequestUserInfoSocial.Instance.RemovePendingFriend(PlayerPrefs.GetString("UserID"), item.friendID);
                                RequestUserInfoSocial.Instance.RemoveSentFriend(PlayerPrefs.GetString("UserID"), item.friendID);
                            }
                        }
                    }
                }
            });
        });

        Transform level = userInfo.transform.Find("level");
        Transform name = userInfo.transform.Find("NameText");
        TextMeshProUGUI nameText = name.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI levelText = level.GetComponentInChildren<TextMeshProUGUI>();

        Transform healthC = currentCritteron.transform.Find("Health");
        Transform levelC = currentCritteron.transform.Find("Level");
        TextMeshProUGUI healthCText = healthC.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI levelCText = levelC.GetComponentInChildren<TextMeshProUGUI>();
        Transform imageCritteron = currentCritteron.transform.Find("Critteron");

        TextMeshProUGUI generalText = generalInfo.GetComponentInChildren<TextMeshProUGUI>();

        RequestUserInfo.Instance.GetUserData(PlayerPrefs.GetString("UserID"), userData =>
        {
            nameText.text = userData.name;
            levelText.text =  "LVL "  + userData.level.ToString();
            RequestUserInfo.Instance.GetUserCritteronsByID(PlayerPrefs.GetString("UserID"), userData.currentCritteron.ToString(), critteron =>
            {
                levelCText.text = critteron.level.ToString();
            });
            RequestGameInfo.Instance.GetCritteronByID(userData.currentCritteron.ToString(), critteron2 =>
            {
                foreach (Transform child in imageCritteron)
                {
                    child.gameObject.SetActive(false);
                }

                Transform targetChild = imageCritteron.Find(critteron2.name);
                targetChild.gameObject.SetActive(true);
                healthCText.text = critteron2.name;
            });
        });

        RequestUserInfo.Instance.GetUserPersonalStat(PlayerPrefs.GetString("UserID"), personalStat =>
        {

            string generalInfoText = $"Steps: {personalStat.globalSteps}\n" +
                            $"Days Streak: {personalStat.daysStreak}\n" +
                            $"Week Steps: {personalStat.weekSteps}\n" +
                            $"Combat Wins: {personalStat.combatWins}";

            generalText.text = generalInfoText;
        });

        RequestUserInfo.Instance.GetUserCritterons(PlayerPrefs.GetString("UserID"), critteronsUserList =>
        {
            RequestGameInfo.Instance.GetAllCritteron(critteronsList =>
            {
                foreach (var critteronInfo in critteronsUserList)
                {
                    if (critteron == null || critteronsRoot == null)
                        break;

                    GameObject newCritteron = Instantiate(critteron, critteronsRoot.transform);

                    ButtonSocial b = newCritteron.GetComponent<ButtonSocial>();
                    b.SetidCritteron(critteronInfo.critteronID);

                    Transform bCritteron = newCritteron.transform.Find("Button");

                    var matchingCritteron = critteronsList.FirstOrDefault(c => c.id == critteronInfo.critteronID);
                    if (matchingCritteron != null)
                    {
                        Transform image = bCritteron.transform.Find(matchingCritteron.name);
                        image.gameObject.SetActive(true);

                        Transform healthDataTransform = newCritteron.transform.Find("Health");
                        TextMeshProUGUI healthText = healthDataTransform.GetComponentInChildren<TextMeshProUGUI>();
                        healthText.text = critteronInfo.currentLife.ToString();

                        Transform data = newCritteron.transform.Find("Data");
                        TextMeshProUGUI dataText = data.GetComponentInChildren<TextMeshProUGUI>();
                        dataText.text = matchingCritteron.name;
                    }

                }
            });
        });
    }


    public void LoadHotel()
    {
        SceneManager.LoadScene("Hotel");
    }
}
