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
       // RequestUserInfoSocial.Instance.ModifySocialStat(PlayerPrefs.GetString("UserID"), "werer");
        RequestUserInfoSocial.Instance.RemoveFriend(PlayerPrefs.GetString("UserID"), "werer");

        //RequestUserInfo.Instance.ModifyUserForniture("674c6f93635e8758d228cd48", "mesa");
    }
}
