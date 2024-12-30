using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSocial : MonoBehaviour
{

    string idCritteron;

    public void SetidCritteron(string id)
    {
        idCritteron = id;
    }

    public void SetCurrentCritteron()
    {
        RequestUserInfo.Instance.ModifyUserData(PlayerPrefs.GetString("UserID"), currentCritteron: idCritteron);
    }
}
