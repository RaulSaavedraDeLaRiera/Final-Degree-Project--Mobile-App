using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Test()
    {
        RequestUserInfo.Instance.ModifyUserRooms(PlayerPrefs.GetString("UserID"), "6755c9cab8d0a120196ac8fe");
        RequestUserInfo.Instance.ModifyUserCritteron(PlayerPrefs.GetString("UserID"), "674f75026a95cc2bedfa50a9");
        //RequestUserInfoSocial.Instance.A(PlayerPrefs.GetString("UserID"), "fasdfasdfsf");
        // RequestUserInfoSocial.Instance.RemoveFriend(PlayerPrefs.GetString("UserID"), "werer");

        //RequestUserInfo.Instance.ModifyUserForniture("674c6f93635e8758d228cd48", "mesa");
    }
}
