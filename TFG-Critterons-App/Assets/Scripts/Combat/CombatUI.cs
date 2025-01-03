using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
                        info.critterons[i].live.ToString() + "/" + info.critterons[i].live.ToString();
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

         string specialAttack1 = "LLAMARADA", specialAttack2 = "MONDONGO";
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
                combat1vs1Health[combatId].text = health.ToString() + "/" + maxHealth.ToString();
                break;
            case CombatType.combat2vs1:
                combat2vs1Health[combatId].text = health.ToString() + "/" + maxHealth.ToString();
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
        attacks[1].color = attacks[2].color = disableAttack;
        Invoke("EnableSpecialAttacks", time);
    }

    void EnableSpecialAttacks()
    {
        attacks[1].color = attacks[2].color = Color.white;

    }
}
