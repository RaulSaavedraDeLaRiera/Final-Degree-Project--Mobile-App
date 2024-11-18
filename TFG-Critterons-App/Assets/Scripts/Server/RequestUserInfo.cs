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

    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="callback"></param>
    public void GetUserByID(string id, System.Action<I_User> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetUserByID(id, callback));
    }

    /// <summary>
    /// Obtiene la lista de todos los usuarios
    /// </summary>
    /// <param name="callback"></param>
    public void GetAllUsers(Action<List<I_User>> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetAllUsersAsync(callback));
    }

    /// <summary>
    /// Obtiene la cantidad de dinero del usuario
    /// </summary>
    /// <param name="id">ID del usuario.</param>
    /// <param name="callback"></param>
    public void GetUserMoney(string id, System.Action<int> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.userData != null)
                callback?.Invoke(user.userData.money);
            else
                callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene el nivel del usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="callback"></param>
    public void GetUserLevel(string id, System.Action<int> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.userData != null)
                callback?.Invoke(user.userData.level);
            else
                callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene la experiencia del usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="callback"></param>
    public void GetUserExperience(string id, System.Action<int> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.userData != null)
                callback?.Invoke(user.userData.experience);
            else
                callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene el ID del Critteron actual del usuario 
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="callback">C</param>
    public void GetUserCurrentCritteron(string id, System.Action<string> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.userData != null)
                callback?.Invoke(user.userData.currentCritteron);
            else
                callback?.Invoke("error");
        });
    }

    /// <summary>
    /// Obtiene el numero total de pasos globales del usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="callback"></param>
    public void GetUserGlobalSteps(string id, System.Action<int> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.personalStats != null)
                callback?.Invoke(user.personalStats.globalSteps);
            else
                callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene el numero de dias consecutivos en racha del usuario 
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="callback"></param>
    public void GetUserDaysStreak(string id, System.Action<int> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.personalStats != null)
                callback?.Invoke(user.personalStats.daysStreak);
            else
                callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene el numero de pasos realizados en la semana actual por el usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="callback"></param>
    public void GetUserWeekSteps(string id, System.Action<int> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.personalStats != null)
                callback?.Invoke(user.personalStats.weekSteps);
            else
                callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene el numero de combates ganados por el usuario
    /// </summary>
    /// <param name="id">ID del usuario.</param>
    /// <param name="callback"></param>
    public void GetUserCombatWins(string id, System.Action<int> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.personalStats != null)
                callback?.Invoke(user.personalStats.combatWins);
            else
                callback?.Invoke(-1);
        });
    }
    /// <summary>
    /// Obtiene la cantidad de Critterons que el usuario posee
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="callback"></param>
    public void GetUserCritteronsOwned(string id, System.Action<int> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.personalStats != null)
                callback?.Invoke(user.personalStats.critteronsOwned);
            else
                callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene el porcentaje de progreso del usuario en el hotel
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="callback">C</param>
    public void GetUserPercentHotel(string id, System.Action<int> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.personalStats != null)
                callback?.Invoke(user.personalStats.percentHotel);
            else
                callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene las estadisticas sociales del usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="callback"></param>
    public void GetUserSocialStats(string id, System.Action<List<I_User.SocialStat>> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.socialStats != null)
                callback?.Invoke(user.socialStats);
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
    /// Obtiene la vida actual de un Critteron 
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="critteronId">ID del Critteron</param>
    /// <param name="callback"></param>
    public void GetCritteronCurrentLife(string userId, string critteronId, System.Action<float> callback)
    {
        GetUserByID(userId, (user) =>
        {
            if (user != null && user.critterons != null)
            {
                var critteron = user.critterons.Find(c => c.critteronID == critteronId);
                if (critteron != null)
                {
                    callback?.Invoke(critteron.currentLife);
                    return;
                }
            }
            callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene el nivel de un Critteron 
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="critteronId">ID del Critteron</param>
    /// <param name="callback"></param>
    public void GetCritteronLevel(string userId, string critteronId, System.Action<int> callback)
    {
        GetUserByID(userId, (user) =>
        {
            if (user != null && user.critterons != null)
            {
                var critteron = user.critterons.Find(c => c.critteronID == critteronId);
                if (critteron != null)
                {
                    callback?.Invoke(critteron.level);
                    return;
                }
            }
            callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene los pasos realizados como compañero por un Critteron 
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="critteronId">ID del Critteron</param>
    /// <param name="callback"></param>
    public void GetCritteronStepAsPartner(string userId, string critteronId, System.Action<int> callback)
    {
        GetUserByID(userId, (user) =>
        {
            if (user != null && user.critterons != null)
            {
                var critteron = user.critterons.Find(c => c.critteronID == critteronId);
                if (critteron != null && critteron.startInfo != null)
                {
                    callback?.Invoke(critteron.startInfo.stepAsPartner);
                    return;
                }
            }
            callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene la cantidad de ataques usados por un Critteron 
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="critteronId">ID del Critteron</param>
    /// <param name="callback"></param>
    public void GetCritteronUsedAttacks(string userId, string critteronId, System.Action<int> callback)
    {
        GetUserByID(userId, (user) =>
        {
            if (user != null && user.critterons != null)
            {
                var critteron = user.critterons.Find(c => c.critteronID == critteronId);
                if (critteron != null && critteron.startInfo != null)
                {
                    callback?.Invoke(critteron.startInfo.usedAttacks);
                    return;
                }
            }
            callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene el numero de combates ganados por un Critteron
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="critteronId">ID del Critteron</param>
    /// <param name="callback"></param>
    public void GetCritteronCombatWins(string userId, string critteronId, System.Action<int> callback)
    {
        GetUserByID(userId, (user) =>
        {
            if (user != null && user.critterons != null)
            {
                var critteron = user.critterons.Find(c => c.critteronID == critteronId);
                if (critteron != null && critteron.startInfo != null)
                {
                    callback?.Invoke(critteron.startInfo.combatWins);
                    return;
                }
            }
            callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene el tiempo en anaerobico registrado por un Critteron
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="critteronId">ID del Critteron</param>
    /// <param name="callback"></param>
    public void GetCritteronTimeAnaerobic(string userId, string critteronId, System.Action<int> callback)
    {
        GetUserByID(userId, (user) =>
        {
            if (user != null && user.critterons != null)
            {
                var critteron = user.critterons.Find(c => c.critteronID == critteronId);
                if (critteron != null && critteron.startInfo != null)
                {
                    callback?.Invoke(critteron.startInfo.timeAnaerobic);
                    return;
                }
            }
            callback?.Invoke(-1);
        });
    }

    /// <summary>
    /// Obtiene el numero de pasos realizados en "parade" de un Critteron 
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="critteronId">ID del Critteron</param>
    /// <param name="callback"></param>
    public void GetCritteronStepsDoingParade(string userId, string critteronId, System.Action<int> callback)
    {
        GetUserByID(userId, (user) =>
        {
            if (user != null && user.critterons != null)
            {
                var critteron = user.critterons.Find(c => c.critteronID == critteronId);
                if (critteron != null && critteron.startInfo != null)
                {
                    callback?.Invoke(critteron.startInfo.stepsDoingParade);
                    return;
                }
            }
            callback?.Invoke(-1);
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

    public void ModifyUserForniture(string id, string newValue)
    {
        StartCoroutine(ServerConnection.Instance.ModifyUserField(id, "furnitureOwned", newValue));
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

    /// <summary>
    /// Modificar cualquier campo de usuario, debemos tener en cuenta que newValue debe de ser correcto dependiendo del campo que usemos
    /// Para campos complejos se recomienda usar los metodos anteriores
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <param name="fieldName"></param>
    /// <param name="newValue"></param>
    public void ModifyUserField<T>(string id, string fieldName, T newValue)
    {
        StartCoroutine(ServerConnection.Instance.ModifyUserField(id, fieldName, newValue));
    }

}
