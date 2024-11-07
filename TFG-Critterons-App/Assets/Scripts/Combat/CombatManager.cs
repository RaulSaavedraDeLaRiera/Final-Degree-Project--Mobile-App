using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField]
    CombatType combatType;
    [SerializeField]
    float turnDuration = 0.5f, attackExtraDamage = 1.25f;
    [SerializeField]
    bool autoAttack = true;

    //1 posicicion critteron 1 en equipo
    //2 posicion critteron 1, 2 en equipo - 3 posicion critteron 2, 2 en equipo
    [SerializeField]
    Transform[] allyCritteronsPos, enemyCritteronsPos;

    [SerializeField]
    List<Critteron> allyCritterons = new List<Critteron>(),
        enemyCritterons = new List<Critteron>();

    int turn = 1;

    AttackSelected attackSelected;

    private void Start()
    {
        EmplaceCritterons();

        InvokeRepeating("Turn", turnDuration, turnDuration);
    }

    void EmplaceCritterons()
    {
        //aqui cargaria los datos de critterons de donde esten guardados

        switch (combatType)
        {
            case CombatType.combat1vs1:
                allyCritterons[0].transform.position = allyCritteronsPos[0].position;
                enemyCritterons[0].transform.position = enemyCritteronsPos[0].position;
                break;
            case CombatType.combat2vs1:
                allyCritterons[0].transform.position = allyCritteronsPos[0].position;
                allyCritterons[1].transform.position = allyCritteronsPos[0].position;
                enemyCritterons[0].transform.position = enemyCritteronsPos[0].position;
                break;
        }
    }

    public void CritteronDefeated(Critteron critteron)
    {
        switch (combatType)
        {
            case CombatType.combat1vs1:

                //final de combate por alguna parte
                CancelInvoke();

                if (enemyCritterons.Contains(critteron))
                {
                    //fin del combate, critteron enemigo derrotado
                    EndCombat(true);
                }
                else
                {
                    //fin del combate, critteron aliado derrotado
                    EndCombat(false);
                }
                break;

            case CombatType.combat2vs1:
                if(enemyCritterons.Contains(critteron))
                {
                    //fin del combate, critteron enemigo derrotado
                    CancelInvoke();

                    EndCombat(true);
                }
                else
                {
                    allyCritterons.Remove(critteron);
                    if(allyCritterons.Count == 0)
                    {
                        //fin del combate
                        CancelInvoke();

                        EndCombat(false);
                    }
                }
                break;
        }
    }


    public void AttackInput(int selected)
    {

        Debug.Log("ataque seleccionado: " + selected + " en turno " + turn);
        attackSelected = (AttackSelected)selected;
    }

    void Turn()
    {

       
        switch (combatType)
        {
            //comnate 1 vs 1, atacan por momentos
            case CombatType.combat1vs1:
                if (turn % 2 != 0 && (autoAttack || attackSelected != AttackSelected.none))
                    allyCritterons[0].Attack(enemyCritterons[0], attackSelected, attackExtraDamage);
                else
                    enemyCritterons[0].Attack(allyCritterons[0]);

                break;


            case CombatType.combat2vs1:
                //atacan en el primer y segundo turno los aliados
                if (turn % 3 != 0)
                {
                    // primero el del jugador
                    if (turn % 2 != 0 && (autoAttack || attackSelected != AttackSelected.none))
                        allyCritterons[0].Attack(enemyCritterons[0], attackSelected, attackExtraDamage);
                    //y luego el del aliado
                    else
                        allyCritterons[1].Attack(enemyCritterons[0]);
                }
                //en el tercero ataca el enemigo
                else
                {
                    //alternando del critteron del jugador al del aliado 
                    if (turn % 2 != 0)
                        enemyCritterons[0].Attack(allyCritterons[0]);
                    else
                        enemyCritterons[0].Attack(allyCritterons[1]);

                }

                break;
        }

        turn++;
        attackSelected = AttackSelected.none;
    }

    void EndCombat(bool win)
    {
        if(win)
        {
            Debug.Log("derrotado enemigos");
        }
        else
        {
            Debug.Log("derrotado aliados");
        }

    }
}

public enum CombatType { combat1vs1, combat2vs1 }
public enum AttackSelected {none, normalAttack, specialAttack1, specialAttack2 }
