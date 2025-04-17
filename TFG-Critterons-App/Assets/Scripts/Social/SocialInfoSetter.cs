using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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


       /* TextMeshProUGUI textComponentID = ID.GetComponentInChildren<TextMeshProUGUI>();
        textComponentID.text = PlayerPrefs.GetString("UserID");*/

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
        List<I_User> allUsers = await RequestUserInfo.Instance.GetAllUsersAsync();

        List<I_User> sortedUsers = allUsers
            .OrderByDescending(u => u.personalStats?.globalSteps ?? 0)
            .ToList();

        for (int i = 0; i < globalRankingUser.Count; i++)
        {
            if (i < sortedUsers.Count)
            {
                I_User user = sortedUsers[i];
                TextMeshProUGUI textComponent = globalRankingUser[i].GetComponentInChildren<TextMeshProUGUI>();

                if (textComponent != null)
                {
                    string name = user.userData.name;
                    int steps = user.personalStats?.globalSteps ?? 0;
                    textComponent.text = $"{i + 1}. {name} - {steps} STEPS";
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
        I_User mainUser = await RequestUserInfo.Instance.GetUserByIDAsync(userId);

        List<string> allUserIds = new List<string> { userId };
       
        for (int i = 0;i < mainUser.socialStats.Count; i++)
        {
            allUserIds.Add(mainUser.socialStats[i].friendID);
        }

        List<I_User> allUsers = new List<I_User>();
        foreach (string id in allUserIds.Distinct())
        {
            I_User user = await RequestUserInfo.Instance.GetUserByIDAsync(id);
            if (user != null)
            {
                allUsers.Add(user);
            }
        }

        // Ordenar por número de pasos (mayor a menor)
        List<I_User> sortedUsers = allUsers
            .OrderByDescending(u => u.personalStats?.globalSteps ?? 0)
            .ToList();

        // Mostrar en UI
        for (int i = 0; i < friendRankingUser.Count; i++)
        {
            if (i < sortedUsers.Count)
            {
                I_User user = sortedUsers[i];

                TextMeshProUGUI textComponent = friendRankingUser[i].GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = $"{user.userData.name} - {user.personalStats?.globalSteps ?? 0} STEPS";
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

