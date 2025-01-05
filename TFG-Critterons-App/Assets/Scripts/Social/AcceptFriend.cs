using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptFriend : MonoBehaviour
{

    string id;

    public void SetID(string id)
    {
        this.id = id;
    }

    public void AddFriend()
    {
        RequestUserInfo.Instance.RemovePendingFriend(PlayerPrefs.GetString("UserID"), id);
        RequestUserInfo.Instance.ModifySocialStat(PlayerPrefs.GetString("UserID"), id);
        RequestUserInfo.Instance.ModifyPendingFriend(id, PlayerPrefs.GetString("UserID"));

        Destroy(transform.gameObject);
    }

    public void DenyFriend()
    {
        RequestUserInfo.Instance.RemovePendingFriend(PlayerPrefs.GetString("UserID"), id);
        Destroy(transform.gameObject);

    }
}
