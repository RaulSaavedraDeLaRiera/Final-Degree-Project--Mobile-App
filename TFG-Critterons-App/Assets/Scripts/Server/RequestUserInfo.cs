using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestUserInfo : MonoBehaviour
{


    private static RequestUserInfo _instance;

    public static RequestUserInfo Instance
    {
        get
        {
            if (_instance == null)
            {
                UnityEngine.Debug.LogError("RequestUserInfo instance is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GetUserByID(string id, System.Action<I_User> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetUserByID(id, callback));
    }

    public void GetAllUsers(Action<List<I_User>> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetAllUsersAsync(callback));
    }

    public void CreateNewUser(string nickname, string password)
    {
        StartCoroutine(ServerConnection.Instance.CreateNewUser(nickname, password));
    }

    public void ModifyUserMoney(string id, int newValue)
    {
        StartCoroutine(ServerConnection.Instance.ModifyUserField(id, "userData.money", newValue));
    }
    public void ModifyUserLevel(string id, int newValue)
    {
        StartCoroutine(ServerConnection.Instance.ModifyUserField(id, "userData.level", newValue));
    }
    public void ModifyUserExperience(string id, int newValue)
    {
        StartCoroutine(ServerConnection.Instance.ModifyUserField(id, "userData.experience", newValue));
    }
    public void ModifyUserCurrentCritteron(string id, string newValue)
    {
        StartCoroutine(ServerConnection.Instance.ModifyUserField(id, "userData.currentCritteron", newValue));
    }

    public void ModifyUserCritteron(string idUser, string idCritteron, int? level = null,
     float? currentLife = null, int? stepAsPartner = null, int? usedAttacks = null,
     int? combatWins = null, int? timeAnaerobic = null, int? stepsDoingParade = null)
    {
        GetUserByID(idUser, (auxUser) =>
        {
            if (auxUser == null)
            {
                Debug.LogError("User not found");
                return;
            }

            I_User.Critteron critteron = auxUser.critterons.Find(c => c.critteronID == idCritteron);

            if (critteron == null)
            {
                critteron = new I_User.Critteron
                {
                    critteronID = idCritteron,
                    level = level ?? 0,
                    currentLife = currentLife ?? 0,
                    startInfo = new I_User.Critteron.StartInfo
                    {
                        stepAsPartner = stepAsPartner ?? 0,
                        usedAttacks = usedAttacks ?? 0,
                        combatWins = combatWins ?? 0,
                        timeAnaerobic = timeAnaerobic ?? 0,
                        stepsDoingParade = stepsDoingParade ?? 0
                    }
                };
            }
            else
            {
                critteron.level = level ?? critteron.level;
                critteron.currentLife = currentLife ?? critteron.currentLife;

                if (critteron.startInfo == null)
                    critteron.startInfo = new I_User.Critteron.StartInfo();

                critteron.startInfo.stepAsPartner = stepAsPartner ?? critteron.startInfo.stepAsPartner;
                critteron.startInfo.usedAttacks = usedAttacks ?? critteron.startInfo.usedAttacks;
                critteron.startInfo.combatWins = combatWins ?? critteron.startInfo.combatWins;
                critteron.startInfo.timeAnaerobic = timeAnaerobic ?? critteron.startInfo.timeAnaerobic;
                critteron.startInfo.stepsDoingParade = stepsDoingParade ?? critteron.startInfo.stepsDoingParade;
            }

            var json = new SimpleJSON.JSONObject
            {
                ["fieldName"] = "critterons",
                ["newValue"] = new SimpleJSON.JSONObject
                {
                    ["critteronID"] = critteron.critteronID,
                    ["level"] = critteron.level,
                    ["currentLife"] = critteron.currentLife,
                    ["startInfo"] = new SimpleJSON.JSONObject
                    {
                        ["stepAsPartner"] = critteron.startInfo.stepAsPartner,
                        ["usedAttacks"] = critteron.startInfo.usedAttacks,
                        ["combatWins"] = critteron.startInfo.combatWins,
                        ["timeAnaerobic"] = critteron.startInfo.timeAnaerobic,
                        ["stepsDoingParade"] = critteron.startInfo.stepsDoingParade
                    }
                }
            };

            StartCoroutine(ServerConnection.Instance.ModifyUserField(idUser, "critterons", json));
        });
    }


    public void ModifyUserField<T>(string id, string fieldName, T newValue)
    {
        StartCoroutine(ServerConnection.Instance.ModifyUserField(id, fieldName, newValue));
    }

}
