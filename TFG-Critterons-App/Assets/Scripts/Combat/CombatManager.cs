using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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

    [SerializeField]
    WaitingAnimation waitingScreen;


    string idCritteronCurrent;
    I_User user;

    bool getExp = true;
    int expCombat;

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

    float coldownSpecialAttack = 2.5f, lastSpecialAttack;

    private void Start()
    {

        lastSpecialAttack = -coldownSpecialAttack;

        expCombat = InfoCache.GetGameInfo().expPerCombat;

        SetCombat();
    }

    async Task SetCombat()
    {
        CritteronCombatInfo[] crittteronsInfo = new CritteronCombatInfo[2];
        string userId = PlayerPrefs.GetString("UserID");


        user = await RequestUserInfo.Instance.GetUserAsync(userId); ;
        idCritteronCurrent = user.userData.currentCritteron;


        var critteron = await RequestUserInfo.Instance.GetUserCritteronsByIDAsync(userId, idCritteronCurrent);
        var critteronGame = await RequestGameInfo.Instance.GetCritteronByIDAsync(idCritteronCurrent);

        var extra = await RequestUserInfo.Instance.GetExtraRoomTypeAsync(userId, 2);
        var experiencieExtra = await RequestUserInfo.Instance.GetExtraRoomTypeAsync(userId, 3);

        // Critteron aliado
        crittteronsInfo[0] = new CritteronCombatInfo(
            (int)critteronGame.life + critteron.level,
            (int)extra + critteronGame.basicDamage + critteron.level / 2,
            critteronGame.name,
            critteron.level,
            (int)critteron.currentLife + critteron.level,
            critteronGame.defense,
            critteronGame,
            critteronGame.attacks[0].damage,
            critteronGame.attacks[1].damage
        );

        var critterons = await RequestGameInfo.Instance.GetAllCritteronAsync();


        List<I_Critteron> list = new List<I_Critteron>();
        foreach (var crit in critterons)
        {
            if (crit.levelUnlock <= user.userData.level)
            {
                list.Add(crit);
            }
        }

        if (PlayerPrefs.GetInt("FriendTogetherCombat", 0) == 1)
        {
            XasuControl.MessageWithCustomVerb(
                actionId: "Combat_dual_Start",
                verbId: "http://adlnet.gov/expapi/verbs/interacted",
                verbDisplay: "interacted",
                timestamp: DateTime.UtcNow
            );

            PlayerPrefs.SetInt("FriendCombat", 0);

            string friendId = PlayerPrefs.GetString("IDFriend");

            var friend = await RequestUserInfo.Instance.GetUserDataAsync(PlayerPrefs.GetString("IDFriend"));

            var idCritteronCurrentFriend = friend.currentCritteron;

            var critteronF = await RequestUserInfo.Instance.GetUserCritteronsByIDAsync(PlayerPrefs.GetString("IDFriend"), idCritteronCurrentFriend);
            var critteronFGame = await RequestGameInfo.Instance.GetCritteronByIDAsync(idCritteronCurrentFriend);


            crittteronsInfo = new CritteronCombatInfo[3];

            crittteronsInfo[0] = new CritteronCombatInfo(
                (int)critteronGame.life + critteron.level,
                (int)extra + critteronGame.basicDamage + critteron.level / 2,
                critteronGame.name,
                critteron.level,
                (int)critteron.currentLife + critteron.level,
                critteronGame.defense,
                critteronGame,
                critteronGame.attacks[0].damage,
                critteronGame.attacks[1].damage
            );

            crittteronsInfo[1] = new CritteronCombatInfo(
                (int)critteronFGame.life,
                (int)critteronFGame.basicDamage,
                critteronFGame.name,
                 user.userData.level,
                (int)critteronFGame.life,
                critteronFGame.defense,
                critteronFGame
            );

            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            int randomLevel = UnityEngine.Random.Range(2, critteron.level + 2);

            crittteronsInfo[2] = new CritteronCombatInfo(
                (list[randomIndex].life + list[randomIndex].life / randomLevel) + user.userData.level,
                (list[randomIndex].basicDamage + list[randomIndex].basicDamage / randomLevel) + user.userData.level,
                list[randomIndex].name,
                randomLevel,
                (list[randomIndex].life + list[randomIndex].life / randomLevel) + user.userData.level,
                list[randomIndex].defense,
                null
            );

            for (int i = 0; i < crittteronsInfo.Length; i++)
                Debug.Log(crittteronsInfo[i].currentLife);


            combatType = CombatType.combat2vs1;

            StartCombat(experiencieExtra, crittteronsInfo);
        }
        else if (PlayerPrefs.GetInt("FriendCombat", 0) == 0)
        {
            XasuControl.MessageWithCustomVerb(
            actionId: "Combat_normal_Start",
            verbId: "http://adlnet.gov/expapi/verbs/interacted",
            verbDisplay: "interacted",
            timestamp: DateTime.UtcNow
        );

            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            int randomLevel = UnityEngine.Random.Range(2, critteron.level + 2);

            // Critteron Enemigo
            crittteronsInfo[1] = new CritteronCombatInfo(
                (list[randomIndex].life - list[randomIndex].life / randomLevel) + user.userData.level,
                (list[randomIndex].basicDamage - list[randomIndex].basicDamage / randomLevel) + user.userData.level,
                list[randomIndex].name,
                randomLevel,
                (list[randomIndex].life - list[randomIndex].life / randomLevel) + user.userData.level,
                list[randomIndex].defense,
                null
            );

            combatType = CombatType.combat1vs1;

            StartCombat(experiencieExtra, crittteronsInfo);
        }
        else
        {
            XasuControl.MessageWithCustomVerb(
                actionId: "Combat_friend_Start",
                verbId: "http://adlnet.gov/expapi/verbs/interacted",
                verbDisplay: "interacted",
                timestamp: DateTime.UtcNow
            );

            PlayerPrefs.SetInt("FriendCombat", 0);
            var friend = await RequestUserInfo.Instance.GetUserDataAsync(PlayerPrefs.GetString("IDFriend"));

            var idCritteronCurrentFriend = friend.currentCritteron;

            var critteronF = await RequestUserInfo.Instance.GetUserCritteronsByIDAsync(PlayerPrefs.GetString("IDFriend"), idCritteronCurrentFriend);
            var critteronFGame = await RequestGameInfo.Instance.GetCritteronByIDAsync(idCritteronCurrentFriend);

            crittteronsInfo[1] = new CritteronCombatInfo(
               critteronFGame.life + critteronF.level,
              critteronFGame.basicDamage + critteronF.level,
               critteronFGame.name,
               critteronF.level,
              critteronFGame.life + critteronF.level + user.userData.level,
               critteronFGame.defense,
               null
           );

            getExp = false;
            combatType = CombatType.combat1vs1;

            StartCombat(experiencieExtra, crittteronsInfo);
        }

    }

    void StartCombat(float experiencieExtra, CritteronCombatInfo[] crittteronsInfo)
    {

        if (waitingScreen != null)
            waitingScreen.Hide();

        coldownSpecialAttack *= 1;
        expCombat += (int)experiencieExtra;

        combatInfo = new CombatParameters(combatType, crittteronsInfo);
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
                if (enemyCritterons.Contains(critteron))
                {
                    //fin del combate, critteron enemigo derrotado
                    CancelInvoke();

                    EndCombat(true);
                }
                else
                {


                    CancelInvoke();

                    EndCombat(false);
                }
                break;
        }
    }

    float lastClick = 0f;
    const float timeBtwClick = 0.3f;

    public void AttackInput(int selected)
    {
        if (Time.timeSinceLevelLoad - lastClick < timeBtwClick)
            return;


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

        lastClick = Time.timeSinceLevelLoad;

        combatUI.SelectAttack(selected - 1);
    }

    int effectType, efffectIndex;
    public void SolicitateEffect(int type, int index)
    {
        effectType = type;
        efffectIndex = index;
        effectsRoot.GetChild(type).gameObject.SetActive(true);

        if (type == 0)
            AudioManager.m.Hit();
        else
            AudioManager.m.HitSpecial();

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
                    if ((int)attackSelected > 1)
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
        AudioManager.m.PlaySound("combat");

        if (win)
        {
            PlayerPrefs.SetInt("Result", 0);
            XasuControl.MessageWithCustomVerb(
                 actionId: "Combat_won",
                 verbId: "https://w3id.org/xapi/seriousgames/verbs/completed",
                 verbDisplay: "completed",
                 timestamp: DateTime.UtcNow
             );


            //cargar experiencia maxima y minima segun combate
            var newExp = GetExpPoints(5, 10);
            var life = GetLifeCritterons();

            RequestUserInfo.Instance.ModifyUserCritteron(PlayerPrefs.GetString("UserID"), idCritteronCurrent, currentLife: life[0], exp: newExp[0]);
            Debug.Log("derrotado enemigos");
            RequestUserInfoSocial.Instance.ModifyPersonalStats(PlayerPrefs.GetString("UserID"), combatWins: user.personalStats.combatWins + 1);


            // Dar experiencia al usuario o subir de nivel
            if (getExp)
            {
                if (user.userData.experience + expCombat >= InfoCache.GetGameInfo().expGoal)
                {


                    RequestUserInfo.Instance.ModifyUserData(PlayerPrefs.GetString("UserID"), level: user.userData.level + 1, experience: 0, money: user.userData.money + 20);

                    RequestUserInfo.Instance.GetUserCritteronsByID(PlayerPrefs.GetString("UserID"), user.userData.currentCritteron, c =>
                    {
                        int exp = c.exp + expCombat;
                        int lvl = c.level;
                        if (exp >= InfoCache.GetGameInfo().expGoal)
                        {
                            exp = 0;
                            lvl++;
                            XasuControl.MessageWithCustomVerb(
                                 actionId: "User_" + user.id + " Accessed Reachable Level_" + lvl++,
                                 verbId: "https://w3id.org/xapi/seriousgames/verbs/accessed",
                                 verbDisplay: "accessed",
                                 timestamp: DateTime.UtcNow
                             );

                        }
                        RequestUserInfo.Instance.ModifyUserCritteron(PlayerPrefs.GetString("UserID"), user.userData.currentCritteron, exp: exp, level: lvl);
                        SceneManager.LoadScene("NewLevel");
                    });
                }
                else
                {
                    RequestUserInfo.Instance.ModifyUserData(PlayerPrefs.GetString("UserID"), experience: user.userData.experience + expCombat);
                    SceneManager.LoadScene("EndCombat");
                }
            }
            else
            {
                SceneManager.LoadScene("EndCombat");
            }
        }
        else
        {
            XasuControl.MessageWithCustomVerb(
                    actionId: "Combat_Lost",
                    verbId: "http://adlnet.gov/expapi/verbs/completed",
                    verbDisplay: "completed",
                    timestamp: DateTime.UtcNow
                );

            PlayerPrefs.SetInt("Result", 1);
            if (getExp)
            {
                var life = GetLifeCritterons();
                RequestUserInfo.Instance.ModifyUserCritteron(PlayerPrefs.GetString("UserID"), idCritteronCurrent, currentLife: life[0]);
            }

            SceneManager.LoadScene("EndCombat");
        }
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
    int[] GetExpPoints(int minPerCombat, int maxPerCombat)
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
                * expCombat * (Math.Min(combatInfo.critterons[i].level / 2, 1)));
        }

        return exp;
    }
}



public enum CombatType { combat1vs1, combat2vs1 }
public enum AttackSelected { none, normalAttack, specialAttack1, specialAttack2 }

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

