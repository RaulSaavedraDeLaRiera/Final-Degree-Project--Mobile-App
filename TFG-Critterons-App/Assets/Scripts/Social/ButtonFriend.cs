using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFriend : MonoBehaviour
{
    GameObject friendProfile;
    GameObject rankingG;
    GameObject rankingF;
    GameObject userInfo;
    GameObject currentCritteron;
    GameObject currentCritteronImage;

    TextMeshProUGUI nameText;
    TextMeshProUGUI userImage;
    TextMeshProUGUI levelTextUser;
    TextMeshProUGUI levelText;
    TextMeshProUGUI healthText;
    TextMeshProUGUI critteronText;
    string ID;


    ButtonActions friendProfileButton;


    public void SetID(string id, GameObject profile, GameObject rG, GameObject rF)
    {
        this.ID = id;
        friendProfile = profile;
        friendProfileButton = profile.GetComponent<ButtonActions>();
        rankingG = rG;
        rankingF = rF;
        userInfo = friendProfile.transform.Find("UserInfo").gameObject;
        currentCritteron = userInfo.transform.Find("CurrentCritteron").gameObject;

        var user = userInfo.transform.Find("User").gameObject;
        nameText = user.transform.Find("NameText/Text").gameObject.GetComponent<TextMeshProUGUI>();
        levelTextUser = user.transform.Find("level/Text").GetComponent<TextMeshProUGUI>();

        levelText = currentCritteron.transform.Find("Level/Text").GetComponent<TextMeshProUGUI>();
        healthText = currentCritteron.transform.Find("Health/Text").GetComponent<TextMeshProUGUI>();
        currentCritteronImage = currentCritteron.transform.Find("CritteronImage").gameObject;
    }


    public void ShowProfile()
    {
        //rankingF.SetActive(false);
        //rankingG.SetActive(false);
        SetProfileInfo();
        Debug.Log("perfil amigo: " + friendProfileButton);
        friendProfileButton.EnterAnimation();
        friendProfile.transform.Find("UserInfo/Vs").gameObject.GetComponent<CombatFriend>().SetID(ID);
        friendProfile.transform.Find("UserInfo/Coop").gameObject.GetComponent<CombatFriend>().SetID(ID);

    }


    void SetProfileInfo()
    {
        RequestUserInfo.Instance.GetUserByID(ID, user =>
        {
            nameText.text = user.userData.name;
            levelTextUser.text = user.userData.level.ToString();


            RequestUserInfo.Instance.GetUserCritteronsByID(ID, user.userData.currentCritteron, critteron =>
            {
                RequestGameInfo.Instance.GetCritteronByID(user.userData.currentCritteron, critteronGame =>
                {
                    foreach (Transform child in currentCritteronImage.transform)
                    {
                        child.gameObject.SetActive(false);
                    }

                    Transform targetChild = currentCritteronImage.transform.Find(critteronGame.name);
                    targetChild.gameObject.SetActive(true);


                    levelText.text = critteron.level.ToString();
                    healthText.text = critteron.currentLife.ToString();
                });


            });
        });
    }


}
