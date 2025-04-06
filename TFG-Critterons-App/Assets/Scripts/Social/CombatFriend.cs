using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatFriend : MonoBehaviour
{
    string ID;

    [SerializeField]
    TextMeshProUGUI walkTogetherText;

    void Start()
    {
        if (walkTogetherText != null)
        {
            bool isTogether = PlayerPrefs.GetInt("FriendTogetherCombat", 0) == 1;
            walkTogetherText.text = isTogether ? "END WALK" : "WALK TOGETHER";
        }
    }

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


    public void ToggleCombatTogether()
    {
        bool isTogether = PlayerPrefs.GetInt("FriendTogetherCombat", 0) == 1;

        PlayerPrefs.SetInt("FriendTogetherCombat", isTogether ? 0 : 1);
        PlayerPrefs.SetString("IDFriend", ID);

        if (walkTogetherText != null)
        {
            walkTogetherText.text = isTogether ? "WALK TOGETHER" : "END WALK";
        }
    }

    public void StopCombatTogether()
    {
        PlayerPrefs.SetInt("FriendTogetherCombat", 0);
    }
}
