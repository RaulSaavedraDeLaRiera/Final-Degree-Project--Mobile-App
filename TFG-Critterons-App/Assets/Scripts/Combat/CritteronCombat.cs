using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritteronCombat : MonoBehaviour
{
    [SerializeField]
    int live, damage;
    [SerializeField]
    CombatManager combatManager;
    [SerializeField]
    Animator animator;

    public void Attack(CritteronCombat target)
    {
        Debug.Log(gameObject.name + " attacks " + target.gameObject.name);
        animator.Play("NormalAttack");
        target.GetDamage(damage);
    }
    public void Attack(CritteronCombat target, AttackSelected attackSelected, float extraDamage)
    {
        Debug.Log(gameObject.name + " attacks " + target.gameObject.name);

        switch (attackSelected)
        {
            case AttackSelected.none:
                target.GetDamage(damage);
                animator.Play("NormalAttack");
                break;
            case AttackSelected.normalAttack:
                target.GetDamage((int)(damage * extraDamage));
                animator.Play("NormalAttack");
                break;
            case AttackSelected.specialAttack1:
                target.GetDamage(damage);
                animator.Play("SpecialAttack");
                break;
            case AttackSelected.specialAttack2:
                target.GetDamage(damage);
                animator.Play("SpecialAttack");
                break;
            default:
                break;
        }

    }

    void GetDamage(int dmg)
    {
        live -= dmg;

        if (live <= 0)
        {
            live = 0;
            animator.Play("Die");
            combatManager.CritteronDefeated(this);
        }
    }
}
