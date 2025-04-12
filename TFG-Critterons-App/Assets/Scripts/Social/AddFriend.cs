using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddFriend : MonoBehaviour
{
    [SerializeField]
    TMP_InputField id;

    [SerializeField]
    Transform contentTransform;
    [SerializeField]
    GameObject friend;
    [SerializeField]
    Scrollbar scrollBar;


    private void Start()
    {
        ScrollBarCreator();
        scrollBar.value = 0;
    }

    public void SendFriendRequest()
    {
        RequestUserInfo.Instance.GetAllUsers(listUser =>
        {
            int i = 0;
            bool findID = false;
            while (i < listUser.Count && !findID)
            {
                if (listUser[i].id == id.text)
                {
                    findID = true;
                }

                i++;
            }

            if (!findID)
                Debug.Log("NO ID FRIEND FOUND");
            else
            {

                XasuControl.MessageWithCustomVerb(
                    actionId: "SEND_ADDFRIEND",
                    verbId: "http://adlnet.gov/expapi/verbs/interacted",
                    verbDisplay: "interacted",
                    timestamp: DateTime.UtcNow
                );

                RequestUserInfoSocial.Instance.ModifyPendingFriend(id.text, PlayerPrefs.GetString("UserID"));
                RequestUserInfoSocial.Instance.ModifySentFriend(PlayerPrefs.GetString("UserID"), id.text);
            }
        });
    }

    void ScrollBarCreatorDemo()
    {
        string[] userList = { "0aaa", "1bbb", "2ccc", "0aaa", "1bbb", "2ccc", "0aaa", "1bbb", "2ccc" };
        foreach (var user in userList)
        {
         
                GameObject newElement = Instantiate(friend, contentTransform);
                newElement.transform.localScale = Vector3.one;
                var aux = newElement.GetComponent<AcceptFriend>();
            aux.SetID(user[0].ToString());

                var textComponent = newElement.GetComponentInChildren<TMPro.TextMeshProUGUI>();

                if (textComponent != null)
                {
                    textComponent.text = user;
                };
        }
    }
    private void ScrollBarCreator()
    {
        RequestUserInfo.Instance.GetUserPendingFriend(PlayerPrefs.GetString("UserID"), pending =>
        {
            foreach (var p in pending)
            {
                RequestUserInfo.Instance.GetUserByID(p.friendID, user =>
                {
                    GameObject newElement = Instantiate(friend, contentTransform);
                    newElement.transform.localScale = Vector3.one;
                    var aux = newElement.GetComponent<AcceptFriend>();
                    aux.SetID(p.friendID);

                    var textComponent = newElement.GetComponentInChildren<TMPro.TextMeshProUGUI>();

                    if (textComponent != null)
                    {
                        textComponent.text = user.userData.name;
                    }
                });
            }
            
        });
    }

}
