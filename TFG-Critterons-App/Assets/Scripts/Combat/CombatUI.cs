using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CombatUI : MonoBehaviour
{
    [SerializeField]
    Transform[] combatsUI;

    [SerializeField]
    TextMeshProUGUI[] combat1vs1Names, combat2vs1Names, combat1vs1Health, combat2vs1Health;
    [SerializeField]
    TextMeshProUGUI specialAttack1, specialAttack2;
    [SerializeField]
    Image[] attacks;
    [SerializeField]
    Color disableAttack, selectedAttack;

    [SerializeField]
    Transform effectTextRoot;
    [SerializeField]
    TextMeshProUGUI[] effectTexts;

    bool specialAttacksDisable = false;

    public bool SpecialSttacksDisable
    {
        get
        {
            return specialAttacksDisable;
        }
    }

    CombatType combatType;
    public void SetUI(CombatParameters info)
    {

        combatType = info.combatType;

        combatsUI[(int)info.combatType].gameObject.SetActive(true);
      

        switch (info.combatType)
        {
            case CombatType.combat1vs1:

                for (int i = 0; i < info.critterons.Length; i++)
                {
                    combat1vs1Names[i].text = info.critteronsName[i] + " LVL " + info.critterons[i].level;
                    combat1vs1Health[i].text =
                        info.critterons[i].currentLife.ToString() + "/" + info.critterons[i].live.ToString();
                }
                break;
            case CombatType.combat2vs1:
                for (int i = 0; i < info.critterons.Length; i++)
                {
                    combat2vs1Names[i].text = info.critteronsName[i] + " LVL " + info.critterons[i].level;
                    combat2vs1Health[i].text =
                        info.critterons[i].live.ToString() + "/" + info.critterons[i].live.ToString();
                }
                break;
            default:
                break;
        }

         string specialAttack1 = "ATCK1", specialAttack2 = "ATCK2";
        if (info.critterons[0].critteron != null)
        {
            specialAttack1 = info.critterons[0].critteron.attacks[0].name;
            specialAttack2 = info.critterons[0].critteron.attacks[1].name;
        }

        this.specialAttack1.text = specialAttack1;
        this.specialAttack2.text = specialAttack2;
    }


    public void ChangeHealth(int combatId, int health, int maxHealth)
    {
        switch (combatType)
        {
            case CombatType.combat1vs1:
                combat1vs1Health[combatId].text = (Mathf.Max(0, health)).ToString() + "/" + maxHealth.ToString();
                break;
            case CombatType.combat2vs1:
                combat2vs1Health[combatId].text = (Mathf.Max(0, health)).ToString() + "/" + maxHealth.ToString();
                break;
            default:
                break;
        }
    }

    public void ResetAttacks(bool abs = false)
    {
        for (int i = 0; i < attacks.Length; i++)
            if (!abs && attacks[i].color != disableAttack)
                attacks[i].color = Color.white;
    }
    public void SelectAttack(int attack)
    {

        for (int i = 0; i < attacks.Length; i++)
            if (attacks[i].color != disableAttack)
                attacks[i].color = Color.white;

        attacks[attack].color = selectedAttack;
    }

   
    public void DisableSpecialAttacks(float time)
    {
        specialAttacksDisable = true;
        attacks[1].color = attacks[2].color = disableAttack;
        Invoke("EnableSpecialAttacks", time);
    }

    public void AttackText(int attack, float turnDur)
    {
        string text = "";
        switch (attack)
        {
            case 1:
                text = "EXTRA DAMAGE!";
                break;
            case 2:
                text = specialAttack1.text;
                break;
            case 3:
                text = specialAttack2.text;
                break;
            default:
                break;
        }

        if (text == "")
            return;

        effectTextRoot.gameObject.SetActive(true);

        foreach (var item in effectTexts)
        {
            item.text = text;
        }

        effectTextRoot.localScale = Vector3.zero;

        effectTextRoot.transform.DOScale(1, turnDur / 3 * 2).onComplete = ()=>
        {
            effectTextRoot.gameObject.SetActive(false);
        };
    }

    void EnableSpecialAttacks()
    {
        specialAttacksDisable = false;
        attacks[1].color = attacks[2].color = Color.white;

    }
}
