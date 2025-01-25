using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatFriend : MonoBehaviour
{
    string ID;

    public void SetID(string ID)
    {
        this.ID = ID;
    }

    public void StartCombatFriend()
    {
        PlayerPrefs.SetInt("FriendCombat", 1);
        PlayerPrefs.SetString("IDFriend", ID);

        SceneManager.LoadScene("Combat");
    }
}
