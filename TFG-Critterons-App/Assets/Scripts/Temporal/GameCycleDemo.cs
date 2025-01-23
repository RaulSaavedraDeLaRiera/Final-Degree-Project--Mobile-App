using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCycleDemo : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nextCombatTimer;
    [SerializeField]
    float minTimeNextcombat, maxTimeNextCombat;

    float nextCombatTime;
    bool canAttack = false;

    private void Start()
    {

        RequestUserInfo.Instance.ModifyUserCritteronLifeTime(PlayerPrefs.GetString("UserID"));
        nextCombatTime = Random.Range(minTimeNextcombat, maxTimeNextCombat);
    }
    private void Update()
    {
        int secondsToCombat = ((int)(nextCombatTime - Time.timeSinceLevelLoad));

        if (secondsToCombat <= 0)
        {
            nextCombatTimer.text = "FIGHT!";
            canAttack = true;
        }
        else
            nextCombatTimer.text = "NEXT COMBAT IN: " + secondsToCombat.ToString() + " SECONDS";
    }

    public void LoadCombat()
    {
        if (canAttack)
            SceneManager.LoadScene("Combat");
    }

}
