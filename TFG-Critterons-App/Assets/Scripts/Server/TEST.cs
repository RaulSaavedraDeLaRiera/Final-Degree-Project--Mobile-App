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
        RequestUserInfo.Instance.ModifyUserData("67491414df78db7b7141401e", money: 100);
    }
}
