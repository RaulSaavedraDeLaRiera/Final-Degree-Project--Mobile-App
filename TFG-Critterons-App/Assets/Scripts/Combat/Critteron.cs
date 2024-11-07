using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Critteron : MonoBehaviour
{
    [SerializeField]
    int live, damage;
    [SerializeField]
    CombatManager combatManager;

    public void Attack(Critteron target)
    {
        Debug.Log(gameObject.name + " attacks " + target.gameObject.name);
        target.GetDamage(damage);
    }
    public void Attack(Critteron target, AttackSelected attackSelected, float extraDamage)
    {
        Debug.Log(gameObject.name + " attacks " + target.gameObject.name);

        switch (attackSelected)
        {
            case AttackSelected.none:
                target.GetDamage(damage);
                break;
            case AttackSelected.normalAttack:
                target.GetDamage((int)(damage * extraDamage));
                break;
            case AttackSelected.specialAttack1:
                target.GetDamage(damage);
                break;
            case AttackSelected.specialAttack2:
                target.GetDamage(damage);
                break;
            default:
                break;
        }
      
    }

    void GetDamage(int dmg)
    {
        live -= dmg;

        if(live <= 0)
        {
            live = 0;
            combatManager.CritteronDefeated(this);
        }
    }
}
