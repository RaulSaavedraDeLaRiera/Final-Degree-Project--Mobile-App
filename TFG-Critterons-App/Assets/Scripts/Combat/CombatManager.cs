using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{


    [SerializeField]
    CombatUI combatUI;    
    [SerializeField]
    float turnDuration = 1.5f, attackExtraDamage = 1.25f, enemy2vs1SizeIncrease = 1.5f, effectsDuration = 1f;
    [SerializeField]
    bool autoAttack = true;
    [SerializeField]
    CombatType combatType;

    //1 posicicion critteron 1 en equipo
    //2 posicion critteron 1, 2 en equipo --- 3 posicion critteron 2, 2 en equipo
    [SerializeField]
    Transform[] allyCritteronsPos, enemyCritteronsPos, cameraPos;
    [SerializeField]
    Transform effectsRoot;

    [SerializeField]
    List<CritteronCombat> allyCritterons = new List<CritteronCombat>(),
        enemyCritterons = new List<CritteronCombat>();

    int turn = 1;

    AttackSelected attackSelected;
    CombatParameters combatInfo;

    float coldownSpecialAttack = 2.5f, lastSpecialAttack = 0f;
    float experiencePerCombat = 10;
    private void Start()
    {
        SetCombat(); 
    }

    void SetCombat()
    {

        //aqui cargariamos la informacion de los critterons como parametros extras
        CritteronCombatInfo[] crittteronsInfo = new CritteronCombatInfo[2];
        crittteronsInfo[0] = new CritteronCombatInfo(6, 3, 0, 3);
        // crittteronsInfo[1] = new CritteronCombatInfo(6, 1, 1, 2);
        crittteronsInfo[1] = new CritteronCombatInfo(15, 1, 1, 2);

        //podriamos tener un modificador de esto por hotel
        coldownSpecialAttack *= 1;
        //modificador externo de experienca por hotel
        experiencePerCombat = 1;
        //

        combatInfo = new CombatParameters(CombatType.combat1vs1, crittteronsInfo);

      
        combatType = combatInfo.combatType;

        string[] names = new string[0];

        switch (combatType)
        {
            case CombatType.combat1vs1:
                names = new string[2];
                effectsRoot = effectsRoot.GetChild(0);

                allyCritterons[0].gameObject.SetActive(true);
                names[0] = allyCritterons[0].InitializateCritteron(this, combatUI, combatInfo.critterons[0], 0);

                enemyCritterons[0].gameObject.SetActive(true);
                names[1] = enemyCritterons[0].InitializateCritteron(this, combatUI, combatInfo.critterons[1], 1);
                break;

            case CombatType.combat2vs1:
                names = new string[3];
                effectsRoot = effectsRoot.GetChild(1);

                allyCritterons[0].gameObject.SetActive(true);
                names[0] = allyCritterons[0].InitializateCritteron(this, combatUI, combatInfo.critterons[0], 0);
                allyCritterons[1].gameObject.SetActive(true);
                names[1] = allyCritterons[1].InitializateCritteron(this, combatUI, combatInfo.critterons[1], 1);

                enemyCritterons[0].gameObject.SetActive(true);
                names[2] = enemyCritterons[0].InitializateCritteron(this, combatUI, combatInfo.critterons[2], 2);
                break;
        }

        combatInfo.critteronsName = names;

        EmplaceCritterons();

        combatUI.SetUI(combatInfo);

        InvokeRepeating("Turn", turnDuration, turnDuration);
    }

    void EmplaceCritterons()
    {

        //se le asignan las posiciones dadas
        switch (combatType)
        {
            case CombatType.combat1vs1:
                Camera.main.transform.position = cameraPos[0].position;
                Camera.main.transform.rotation = cameraPos[0].rotation;

                allyCritterons[0].transform.position = allyCritteronsPos[0].position;
                allyCritterons[0].transform.rotation = allyCritteronsPos[0].rotation;
                enemyCritterons[0].transform.position = enemyCritteronsPos[0].position;
                enemyCritterons[0].transform.rotation = enemyCritteronsPos[0].rotation;
                break;
            case CombatType.combat2vs1:
                Camera.main.transform.position = cameraPos[1].position;
                Camera.main.transform.rotation = cameraPos[1].rotation;

                allyCritterons[0].transform.position = allyCritteronsPos[1].position;
                allyCritterons[0].transform.rotation = allyCritteronsPos[1].rotation;
                allyCritterons[1].transform.position = allyCritteronsPos[2].position;
                allyCritterons[1].transform.rotation = allyCritteronsPos[2].rotation;
                enemyCritterons[0].transform.position = enemyCritteronsPos[1].position;
                enemyCritterons[0].transform.rotation = enemyCritteronsPos[1].rotation;
                enemyCritterons[0].transform.localScale *= enemy2vs1SizeIncrease;
                break;
        }
    }

    public void CritteronDefeated(CritteronCombat critteron)
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
        if ((AttackSelected)selected != AttackSelected.normalAttack)
        {
            if (lastSpecialAttack + coldownSpecialAttack >= Time.timeSinceLevelLoad)
                return;
            else
            {
                lastSpecialAttack = Time.timeSinceLevelLoad;
            }
        }

        Debug.Log("ataque seleccionado: " + selected + " en turno " + turn);
        attackSelected = (AttackSelected)selected;

        combatUI.SelectAttack(selected-1);
    }

    int effectType, efffectIndex;
    public void SolicitateEffect(int type, int index)
    {
        effectType = type;
        efffectIndex = index;
        effectsRoot.GetChild(type).gameObject.SetActive(true);
        Invoke("DisableEffect", effectsDuration);
    }

    void DisableEffect()
    {
        effectsRoot.GetChild(effectType).gameObject.SetActive(false);
    }

    void Turn()
    {

       
        switch (combatType)
        {
            //comnate 1 vs 1, atacan por momentos
            case CombatType.combat1vs1:
                if (turn % 2 != 0 && (autoAttack || attackSelected != AttackSelected.none))
                {
                    allyCritterons[0].Attack(enemyCritterons[0], attackSelected, attackExtraDamage);

                    combatUI.ResetAttacks();
                    if((int)attackSelected > 1)
                        combatUI.DisableSpecialAttacks(coldownSpecialAttack);
                }
                else
                    enemyCritterons[0].Attack(allyCritterons[0]);

                break;


            case CombatType.combat2vs1:
                //atacan en el primer y segundo turno los aliados
                if (turn % 3 != 0)
                {
                    // primero el del jugador
                    if (turn % 2 != 0 && (autoAttack || attackSelected != AttackSelected.none))
                    {
                        allyCritterons[0].Attack(enemyCritterons[0], attackSelected, attackExtraDamage);

                        combatUI.ResetAttacks();
                        if ((int)attackSelected > 1)
                            combatUI.DisableSpecialAttacks(coldownSpecialAttack);
                    }
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

        combatUI.ResetAttacks(true);

        if(win)
        {
            //cargar experiencia maxima y minima segun combate
            var exp = GetExpPoints(5, 10);
            var life = GetLifeCritterons();
            //guardar datos
            Debug.Log("derrotado enemigos");
        }
        else
        {
            //vida 0
            var life = GetLifeCritterons();
            //guardar datos
            Debug.Log("derrotado aliados");
        }

        SceneManager.LoadScene("DemoPrincipal");

    }

    int[] GetLifeCritterons()
    {
        int[] life;
        switch (combatType)
        {
            default:
            case CombatType.combat1vs1:
                life = new int[1];
                break;
            case CombatType.combat2vs1:
                life = new int[2];
                break;
        }

        for (int i = 0; i < life.Length; i++)
        {
            life[i] = allyCritterons[i].Health;
        }

        return life;
    }
    int [] GetExpPoints(int minPerCombat, int maxPerCombat)
    {
        int[] exp;
        switch (combatType)
        {
            default:
            case CombatType.combat1vs1:
                exp = new int[1];
                break;
            case CombatType.combat2vs1:
                exp = new int[2];
                break;
        }

        for (int i = 0; i < exp.Length; i++)
        {
            exp[i] = (int)(UnityEngine.Random.Range(minPerCombat, maxPerCombat)
                * experiencePerCombat * (Math.Min(combatInfo.critterons[i].level / 2, 1)));
        }

        return exp;
    }
}

public enum CombatType { combat1vs1, combat2vs1 }
public enum AttackSelected {none, normalAttack, specialAttack1, specialAttack2 }

public struct CombatParameters
{
    public CombatType combatType;
    public CritteronCombatInfo[] critterons;
    public string[] critteronsName;

    public CombatParameters(CombatType combatType, CritteronCombatInfo[] critterons)
    {
        this.combatType = combatType;
        this.critterons = critterons;
        critteronsName = new string[0];
    }
}

