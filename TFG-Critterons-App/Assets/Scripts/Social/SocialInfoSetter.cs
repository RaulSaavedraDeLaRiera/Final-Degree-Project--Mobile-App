using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    List<GameObject> globalRankingUser;

    [SerializeField]
    List<GameObject> friendRankingUser;

    [SerializeField]
    GameObject friendProfile;
    [SerializeField]
    GameObject ID;



    void Start()
    {
        PlayerPrefs.SetInt("FriendCombat", 0);
        PlayerPrefs.SetInt("FriendTogetherCombat", 0);


        TextMeshProUGUI textComponentID = ID.GetComponentInChildren<TextMeshProUGUI>();
        textComponentID.text = PlayerPrefs.GetString("UserID");

        LoadRankingGlobal();
        LoadRankingFriends(PlayerPrefs.GetString("UserID"));

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


    public async void LoadRankingGlobal()
    {
        List<string> listUser = await RequestUserInfo.Instance.GetTopThreeGlobalAsync(); 

        for (int i = 0; i < globalRankingUser.Count; i++)
        {
            if (i < listUser.Count)
            {
                I_User user = await RequestUserInfo.Instance.GetUserByIDAsync(listUser[i]);

                TextMeshProUGUI textComponent = globalRankingUser[i].GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = user.userData.name;
                }

                globalRankingUser[i].SetActive(true);
                globalRankingUser[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                globalRankingUser[i].SetActive(false);
                globalRankingUser[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public async void LoadRankingFriends(string userId)
    {
        List<string> listUser = await RequestUserInfo.Instance.GetTopThreeFriendsByIdAsync(userId);

        for (int i = 0; i < friendRankingUser.Count; i++)
        {
            if (i < listUser.Count)
            {
                I_User user = await RequestUserInfo.Instance.GetUserByIDAsync(listUser[i]);

                TextMeshProUGUI textComponent = friendRankingUser[i].GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = user.userData.name;
                }

                friendRankingUser[i].SetActive(true);
                friendRankingUser[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                friendRankingUser[i].SetActive(false);
                friendRankingUser[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
}

