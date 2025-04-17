using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class InfoPrincipalManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI levelT, expT;


    void Start()
    {
        LoadUserInfoAsync();

    }

    private async Task LoadUserInfoAsync()
    {
        int expGoal = RequestGameInfo.Instance.GetExpGoal();
        var user = await RequestUserInfo.Instance.GetUserAsync(PlayerPrefs.GetString("UserID"));

        int level = user.userData.level;         
        int exp = user.userData.experience;
        int expToLevelUp = expGoal;

        levelT.text = level.ToString();
        expT.text = exp.ToString() + "/" + expToLevelUp.ToString();
    }
}
