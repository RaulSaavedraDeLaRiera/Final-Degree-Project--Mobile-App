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
