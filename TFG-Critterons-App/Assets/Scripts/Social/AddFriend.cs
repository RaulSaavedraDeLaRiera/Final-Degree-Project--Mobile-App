using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AddFriend : MonoBehaviour
{
    [SerializeField]
    TMP_InputField id;

    [SerializeField]
    Transform contentTransform;
    [SerializeField]
    GameObject friend;


    private void Start()
    {
        ScrollBarCreator();
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
                RequestUserInfo.Instance.ModifyPendingFriend(id.text, PlayerPrefs.GetString("UserID"));
                RequestUserInfo.Instance.ModifySentFriend(PlayerPrefs.GetString("UserID"), id.text);
            }
        });
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
