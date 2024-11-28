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

    public void GetUserData(string id, System.Action<I_User.UserData> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.userData != null)
                callback?.Invoke(user.userData);
            else
                callback?.Invoke(null);
        });
    }

    /// <summary>
    /// Obtiene la lista de Critterons del usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="callback"></param>
    public void GetUserCritterons(string id, System.Action<List<I_User.Critteron>> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.critterons != null)
                callback?.Invoke(user.critterons);
            else
                callback?.Invoke(null);
        });
    }

    /// <summary>
    /// Obtiene un critteron en especifico del usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="critteronId">ID del Critteron</param>
    /// <param name="callback"></param>
    public void GetUserCritteronsByID(string userId, string critteronId, System.Action<I_User.Critteron> callback)
    {
        GetUserByID(userId, (user) =>
        {
            if (user != null && user.critterons != null)
            {
                var critteron = user.critterons.Find(c => c.critteronID == critteronId);
                if (critteron != null)
                {
                    callback?.Invoke(critteron);
                    return;
                }
            }
            callback?.Invoke(null);
        });
    }

    public void GetUserFurnitureOwned(string id, System.Action<List<I_User.FurnitureOwned>> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.userData != null)
                callback?.Invoke(user.furnitureOwned);
            else
                callback?.Invoke(null);
        });
    }

    /// <summary>
    /// Crea un nuevo usuario con el nombre de usuario y contraseña proporcionados
    /// </summary>
    /// <param name="nickname">Nombre de usuario</param>
    /// <param name="password">Contraseña</param>
    public void CreateNewUser(string nickname, string password)
    {
        StartCoroutine(ServerConnection.Instance.CreateNewUser(nickname, password));
    }

    public void ModifyUserForniture(string id, string newValue)
    {
        StartCoroutine(ServerConnection.Instance.ModifyUserField(id, "furnitureOwned", newValue));
    }

    public void ModifyUserData(string idUser, string? nickname = null, int? level = null,
    int? experience = null, int? money = null, string? currentCritteron = null)
    {
        GetUserByID(idUser, (auxUser) =>
        {
            if (auxUser == null)
            {
                Debug.LogError("User not found");
                return;
            }

            var currentData = auxUser.userData;

            var newValue = new SimpleJSON.JSONObject
            {
                ["name"] = nickname ?? currentData.name,
                ["level"] = level ?? currentData.level,
                ["experience"] = experience ?? currentData.experience,
                ["money"] = money ?? currentData.money,
                ["currentCritteron"] = currentCritteron ?? currentData.currentCritteron
            };


            StartCoroutine(ServerConnection.Instance.ModifyUserField(idUser, "userData", newValue));
        });
    }

    /// <summary>
    /// Metodo para modificar o añadir informacion sobre los critterons del jugador
    /// </summary>
    /// <param name="idUser"></param>
    /// <param name="idCritteron"></param>
    /// <param name="level"></param>
    /// <param name="currentLife"></param>
    /// <param name="stepAsPartner"></param>
    /// <param name="usedAttacks"></param>
    /// <param name="combatWins"></param>
    /// <param name="timeAnaerobic"></param>
    /// <param name="stepsDoingParade"></param>
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

            // Critteron nuevo para el usuario
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
            // Ya tenia el critteron cambiamos la variables nuevas
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

            var jsonObject = new SimpleJSON.JSONObject();
            jsonObject["critteronID"] = critteron.critteronID;
            jsonObject["level"] = critteron.level;
            jsonObject["currentLife"] = critteron.currentLife;

            var startInfo = new SimpleJSON.JSONObject();
            startInfo["stepAsPartner"] = critteron.startInfo.stepAsPartner;
            startInfo["usedAttacks"] = critteron.startInfo.usedAttacks;
            startInfo["combatWins"] = critteron.startInfo.combatWins;
            startInfo["timeAnaerobic"] = critteron.startInfo.timeAnaerobic;
            startInfo["stepsDoingParade"] = critteron.startInfo.stepsDoingParade;

            jsonObject["startInfo"] = startInfo;

            StartCoroutine(ServerConnection.Instance.ModifyUserField(idUser, "critterons", jsonObject));
        });
    }



}
