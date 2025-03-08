using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCycleDemo : MonoBehaviour
{
    [SerializeField]
    CombatTriggerSystem triggerCombatType;
    [SerializeField]
    TextMeshProUGUI nextCombatInfoText;
    [SerializeField]
    float minTimeNextcombat, maxTimeNextCombat;
    [SerializeField]
    int stepsToCombat = 15;

    float nextCombatTime;
    bool canAttack = false;

    StepCounterV2 stepCounter;
    int stepCountInLastCombat = 0;

    private void Start()
    {

        //RequestUserInfo.Instance.ModifyUserCritteronLifeTime(PlayerPrefs.GetString("UserID"));

        CombatCycle();
    }

    private void Update()
    {
        switch (triggerCombatType)
        {
            case CombatTriggerSystem.time:
                TimeTriggerUpdate();
                break;
            case CombatTriggerSystem.steps:
                StepTriggerUpdate();
                break;
            default:
                break;
        }
    }

    void CombatCycle()
    {
        switch (triggerCombatType)
        {
            case CombatTriggerSystem.time:
                nextCombatTime = Random.Range(minTimeNextcombat, maxTimeNextCombat);
                break;
            case CombatTriggerSystem.steps:
                stepCounter = GameObject.Find("InfoManager").GetComponent<StepCounterV2>();
                break;
        }
    }

    void TimeTriggerUpdate()
    {
        int secondsToCombat = ((int)(nextCombatTime - Time.timeSinceLevelLoad));

        if (secondsToCombat <= 0)
        {
            nextCombatInfoText.text = "FIGHT!";
            canAttack = true;
        }
        else
            nextCombatInfoText.text = "NEXT COMBAT IN: " + secondsToCombat.ToString() + " SECONDS";
    }

    void StepTriggerUpdate()
    {
        int steps = stepCounter.Steps - stepCountInLastCombat;

        //Debug.Log("Steps: " + steps.ToString());

        if(steps >= stepsToCombat)
        {
            nextCombatInfoText.text = "FIGHT! " + steps.ToString();
            canAttack = true;
        }
        else
            nextCombatInfoText.text = "NEXT COMBAT IN: " + (stepsToCombat - steps).ToString() + " STEPS";

    }

    public void LoadCombat()
    {
        if (canAttack)
        {
            canAttack = false;
            SceneManager.LoadScene("Combat");
        }
    }

    enum CombatTriggerSystem { time, steps }
}

