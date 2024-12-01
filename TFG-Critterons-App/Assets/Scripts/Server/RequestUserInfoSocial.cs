using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static I_User;

public class RequestUserInfoSocial : MonoBehaviour
{
    private static RequestUserInfoSocial _instance;

    public static RequestUserInfoSocial Instance
    {
        get
        {
            if (_instance == null)
            {
                UnityEngine.Debug.LogError("RequestUserInfoSocial instance is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void GetUserByID(string id, System.Action<I_User> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetUserByID(id, callback));
    }

    public void GetUserPersonalStats(string id, System.Action<I_User.PersonalStats> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.userData != null)
                callback?.Invoke(user.personalStats);
            else
                callback?.Invoke(null);
        });
    }

    public void GetUserSocialStat(string id, System.Action<List<I_User.SocialStat>> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.userData != null)
                callback?.Invoke(user.socialStats);
            else
                callback?.Invoke(null);
        });
    }

    public void ModifyPersonalStats(string idUser, int? globalSteps = null, int? daysStreak = null, int? weekSteps = null,
        int? combatWins = null, int? critteronsOwned = null, int? percentHotel = null)
    {
        GetUserByID(idUser, (auxUser) =>
        {
            if (auxUser == null)
            {
                Debug.LogError("User not found");
                return;
            }

            var currentData = auxUser.personalStats;

            var newValue = new SimpleJSON.JSONObject
            {
                ["globalSteps"] = globalSteps ?? currentData.globalSteps,
                ["daysStreak"] = daysStreak ?? currentData.daysStreak,
                ["weekSteps"] = weekSteps ?? currentData.weekSteps,
                ["combatWins"] = combatWins ?? currentData.combatWins,
                ["critteronsOwned"] = critteronsOwned ?? currentData.critteronsOwned,
                ["percentHotel"] = percentHotel ?? currentData.percentHotel,
            };
            StartCoroutine(ServerConnection.Instance.ModifyUserField(idUser, "personalStats", newValue));
        });
    }

    public void RemoveFriend(string userId, string friendID)
    {
        GetUserSocialStat(userId, (socialStats) =>
        {
            if (socialStats == null)
            {
                Debug.LogError("User or SocialStats not found");
                return;
            }

            if (!socialStats.Exists(stat => stat.friendID == friendID))
            {
                Debug.LogWarning("Friend not found in the list");
                return;
            }

            var removeFriendJson = new JSONObject();
            removeFriendJson.Add("friendID", new JSONString(friendID));
            StartCoroutine(ServerConnection.Instance.RemoveUserFriend(userId, removeFriendJson));
        });
    }

    public void ModifySocialStat(string id, string newValue)
    {
        StartCoroutine(ServerConnection.Instance.ModifyUserField(id, "socialStats", newValue));
    }

}
