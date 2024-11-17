using System;
using System.Collections.Generic;

[Serializable]
public class I_UserInfo
{
    public string id;
    public List<User> users;

    [Serializable]
    public class User
    {
        public string userID;
    }
}