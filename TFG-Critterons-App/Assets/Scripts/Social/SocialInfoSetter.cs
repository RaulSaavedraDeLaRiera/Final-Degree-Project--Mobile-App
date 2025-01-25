using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SocialInfoSetter : MonoBehaviour
{
    [SerializeField]
    GameObject friend;
    [SerializeField]
    Transform friendList;
    [SerializeField]
    GameObject friendRanking;
    [SerializeField]
    GameObject globalRanking;

    [SerializeField]
    GameObject friendProfile;


    void Start()
    {

        RequestUserInfoSocial.Instance.GetUserSocialStat(PlayerPrefs.GetString("UserID"), friends =>
        {
            foreach (var p in friends)
            {
                RequestUserInfo.Instance.GetUserByID(p.friendID, user =>
                {
                    GameObject newElement = Instantiate(friend, friendList);
                    newElement.transform.localScale = Vector3.one;
                    newElement.GetComponent<ButtonFriend>().SetID(p.friendID, friendProfile, globalRanking, friendRanking);
                    var textComponent = newElement.GetComponentInChildren<TMPro.TextMeshProUGUI>();

                    if (textComponent != null)
                    {
                        textComponent.text = user.userData.name;
                    }
                });
            }

        });
    }

    void AddNewFriend()
    {
        SceneManager.LoadScene("AddFriend");
    }

    public void ClosePopUp()
    {
        friendProfile.SetActive(false);
        friendRanking.SetActive(true);
        globalRanking.SetActive(true);
    }
}
