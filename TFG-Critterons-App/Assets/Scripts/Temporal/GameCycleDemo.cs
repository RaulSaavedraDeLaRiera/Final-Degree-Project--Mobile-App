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


    private void Start()
    {
        nextCombatTime = Random.Range(minTimeNextcombat, maxTimeNextCombat);
    }
    private void Update()
    {

        int secondsToCombat = ((int)(nextCombatTime - Time.timeSinceLevelLoad));

        if (secondsToCombat <= 0)
            LoadCombat();
        else
            nextCombatTimer.text = "NEXT COMBAT IN: " + secondsToCombat.ToString() + " SECONDS";
    }

    void LoadCombat()
    {
        SceneManager.LoadScene("Combat");
    }

}
