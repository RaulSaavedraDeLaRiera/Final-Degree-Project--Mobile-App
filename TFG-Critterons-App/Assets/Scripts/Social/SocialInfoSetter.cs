using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SocialInfoSetter : MonoBehaviour
{
    [SerializeField]
    GameObject friend;
    [SerializeField]
    GameObject friendList;
    [SerializeField]
    GameObject friendRanking;
    [SerializeField]
    GameObject globalRanking;

    void Start()
    {

        //RequestUserInfo.Instance.(listUser =>
        //{
        //    int i = 0;
        //    bool findID = false;
        //    while (i < listUser.Count && !findID)
        //    {
        //        if (listUser[i].id == id.text)
        //        {
        //            findID = true;
        //        }

        //        i++;
        //    }

        //    if (!findID)
        //        Debug.Log("NO ID FRIEND FOUND");
        //    else
        //    {
        //        RequestUserInfo.Instance.ModifyPendingFriend(id.text, PlayerPrefs.GetString("UserID"));
        //        RequestUserInfo.Instance.ModifySentFriend(PlayerPrefs.GetString("UserID"), id.text);
        //    }
        //});

    }

    void AddNewFriend()
    {
        SceneManager.LoadScene("AddFriend");
    }

}
