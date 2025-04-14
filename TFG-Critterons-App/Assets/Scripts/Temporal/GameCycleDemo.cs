using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCycleDemo : MonoBehaviour
{
    [SerializeField]
    CombatTriggerSystem triggerCombatType;
    [SerializeField]
    TextMeshProUGUI nextCombatInfoText;
    [SerializeField]
    float minTimeNextcombat, maxTimeNextCombat;
    [SerializeField]
    int stepsToCombat = 50; 
    [SerializeField]
    Sprite combatEnableSprite;
    [SerializeField]
    Image combatImage;

    float nextCombatTime;
    bool canAttack = false;

    StepCounterV2 stepCounter;
    int stepsInLastCombat;

    private void Start()
    {
        CombatCycle();

        // Cargar los pasos del último combate
        if (PlayerPrefs.HasKey("LastCombatSteps"))
        {
            stepsInLastCombat = PlayerPrefs.GetInt("LastCombatSteps");
            nextCombatInfoText.text += "\nLAST COMBAT: " + stepsInLastCombat.ToString() + " STEPS";
        }
        else
            stepsInLastCombat = 0;

        if(InfoCache.GetGameInfo().stepsToCombat != null)
            stepsToCombat = InfoCache.GetGameInfo().stepsToCombat;

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
            combatImage.sprite = combatEnableSprite;
            canAttack = true;
        }
        else
            nextCombatInfoText.text = "NEXT COMBAT IN: " + secondsToCombat.ToString() + " SECONDS";
    }

    void StepTriggerUpdate()
    {
        int steps = stepCounter.Steps - stepsInLastCombat;

        if (steps >= stepsToCombat)
        {
            nextCombatInfoText.text = "FIGHT!";
            combatImage.sprite = combatEnableSprite;
            canAttack = true;
        }
        else
        {
            nextCombatInfoText.text = "NEXT COMBAT IN: " + (stepsToCombat - steps).ToString() + " STEPS";
        }
    }

    public void LoadCombat()
    {
        if (canAttack)
        {
            canAttack = false;

            stepsInLastCombat = stepCounter.Steps;
            PlayerPrefs.SetInt("LastCombatSteps", stepsInLastCombat);
            PlayerPrefs.Save();

            AudioManager.m.PlaySound("click");

            SceneManager.LoadScene("Combat");
        }
    }

    enum CombatTriggerSystem { time, steps }
}
