using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestUserInfo : MonoBehaviour
{
    private static RequestUserInfo _instance;

    public static RequestUserInfo Instance
    {
        get
        {
            if (_instance == null)
            {
                UnityEngine.Debug.LogError("RequestUserInfo instance is null");
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

    public I_User GetUserByID(string ID)
    {
        return ServerConnection.Instance.GetUserByID(ID);
    }

    public List<I_User> GetAllUser()
    {
        return ServerConnection.Instance.GetAllUsers();
    }
}
