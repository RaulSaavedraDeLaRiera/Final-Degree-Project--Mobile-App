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
    float defense;
    int maxHealth;
    public int Health => health;

    int specialAttack1Damage, specialAttack2Damage;

    public string InitializateCritteron(CombatManager manager, CombatUI ui,
        CritteronCombatInfo info, int combatID)
    {
        combatManager = manager;
        this.ui = ui;
        this.combatID = combatID;

        maxHealth = info.live;
        health = info.currentLife;
        damage = info.damage;
        specialAttack1Damage = info.specialAttack1Damage;
        specialAttack2Damage = info.specialAttack2Damage;
        defense = info.defense;

        visualRoot.Find(info.creature).gameObject.SetActive(true);
        animator = visualRoot.Find(info.creature).GetComponent<Animator>();

        return visualRoot.Find(info.creature).gameObject.name;
    }
   

    public void Attack(CritteronCombat target)
    {
        //Debug.Log(gameObject.name + " attacks " + target.gameObject.name);
        animator.Play("NormalAttack");
        combatManager.SolicitateEffect(0, 0);
        target.GetDamage(0);

    }
    public void Attack(CritteronCombat target, AttackSelected attackSelected, float extraDamage)
    {
        Debug.Log(gameObject.name + " attacks with attack " + attackSelected);

        switch (attackSelected)
        {
            case AttackSelected.none:
                target.GetDamage(damage);
                animator.Play("NormalAttack");
                combatManager.SolicitateEffect(0, 0);
                break;
            case AttackSelected.normalAttack:
                target.GetDamage((int)(damage * extraDamage));
                combatManager.SolicitateEffect(0, 0);
                animator.Play("NormalAttack");
                break;
            case AttackSelected.specialAttack1:
                //se deberia de dar el index respecto a su ataque especial
                target.GetDamage((int)(specialAttack1Damage * extraDamage));
                combatManager.SolicitateEffect(1, 0);
                animator.Play("NormalAttack");
                break;
            case AttackSelected.specialAttack2:
                target.GetDamage((int)(specialAttack2Damage * extraDamage));
                combatManager.SolicitateEffect(1, 0);
                animator.Play("NormalAttack");
                break;
            default:
                break;
        }

    }

    void GetDamage(int dmg)
    {
        health -= dmg - (int)(dmg * defense);

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
    public float defense;
    public int level;
    public int specialAttack1Damage;
    public int specialAttack2Damage;
    public I_Critteron critteron;

    public CritteronCombatInfo(int live, int damage, string creature,int level, int currentLife, float defense, I_Critteron critteron, int specialAttack1Damage = 0, int specialAttack2Damage = 0)
    {
        this.currentLife = currentLife;
        this.live = live;
        this.damage = damage;
        this.creature = creature;
        this.level = level;
        this.critteron = critteron;
        this.defense = defense;
        this.specialAttack1Damage = specialAttack1Damage;
        this.specialAttack2Damage = specialAttack2Damage;
    }

}
