using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class CritteronCombat : MonoBehaviour
{
    [SerializeField]
    int health, damage;
    [SerializeField]
    Transform visualRoot;


    Animator animator;
    CombatManager combatManager;
    CombatUI ui;
    int combatID;
    int defense;
    int maxHealth;
    public int Health => health;


    public string InitializateCritteron(CombatManager manager, CombatUI ui,
        CritteronCombatInfo info, int combatID)
    {
        combatManager = manager;
        this.ui = ui;
        this.combatID = combatID;

        maxHealth = info.live;
        health = info.currentLife;
        damage = info.damage;
        defense = info.defense;

        visualRoot.Find(info.creature).gameObject.SetActive(true);
        animator = visualRoot.Find(info.creature).GetComponent<Animator>();

        return visualRoot.Find(info.creature).gameObject.name;
    }
   

    public void Attack(CritteronCombat target)
    {
        Debug.Log(gameObject.name + " attacks " + target.gameObject.name);
        animator.Play("NormalAttack");
        combatManager.SolicitateEffect(0, 0);
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
                combatManager.SolicitateEffect(0, 0);
                break;
            case AttackSelected.normalAttack:
                combatManager.SolicitateEffect(0, 0);
                target.GetDamage((int)(damage * extraDamage));
                animator.Play("NormalAttack");
                break;
            case AttackSelected.specialAttack1:
                //se deberia de dar el index respecto a su ataque especial
                combatManager.SolicitateEffect(1, 0);
                target.GetDamage(damage);
                animator.Play("SpecialAttack");
                break;
            case AttackSelected.specialAttack2:
                combatManager.SolicitateEffect(1, 0);
                target.GetDamage(damage);
                animator.Play("SpecialAttack");
                break;
            default:
                break;
        }

    }

    void GetDamage(int dmg)
    {
        if (damage > defense )
        {
            damage -= defense / 2;
        }
        else if (damage < 0) 
        {
            damage /= 2;
        }

        health -= dmg;

        ui.ChangeHealth(combatID, health, maxHealth);

        if (health <= 0)
        {
            health = 0;
            animator.Play("Die");
            combatManager.CritteronDefeated(this);
        }
    }
}

public struct CritteronCombatInfo{
    public int live;
    public int currentLife;
    public int damage;
    public string creature;
    public int defense;
    public int level;
    public I_Critteron critteron;

    public CritteronCombatInfo(int live, int damage, string creature,int level, int currentLife, int defense,I_Critteron critteron)
    {
        this.currentLife = currentLife;
        this.live = live;
        this.damage = damage;
        this.creature = creature;
        this.level = level;
        this.critteron = critteron;
        this.defense = defense;
    }

}
