using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

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

    private void OnApplicationQuit()
    {
        if (PlayerPrefs.GetString("UserID") != null)
        {
            ModifyUserTime(PlayerPrefs.GetString("UserID"));
        }

    }

    private void OnApplicationPause(bool pauseStatus)
    {
    }

    private void OnApplicationFocus(bool hasFocus)
    {


    }

    public void Login(string mail, string password, Action<bool> onLoginComplete)
    {
        StartCoroutine(ServerConnection.Instance.LoginToken(mail, password, (token) =>
        {
            if (token != "")
            {
                PlayerPrefs.SetString("token", token);
                PlayerPrefs.Save();
                Debug.Log($"Token received: {token}");

                StartCoroutine(ServerConnection.Instance.GetIDUser(mail, password, (id) =>
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        PlayerPrefs.SetString("UserID", id);
                        PlayerPrefs.Save();
                    }
                    else
                    {
                        Debug.LogError("Failed to retrieve User ID.");
                    }
                }));

                onLoginComplete?.Invoke(true);
            }
            else
            {
                Debug.Log("Server error: Unable to login");
                onLoginComplete?.Invoke(false);
            }
        }));
    }

    public void GetUserByID(string id, System.Action<I_User> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetUserByID(id, callback));
    }
    public Task<I_User> GetUserByIDAsync(string id)
    {
        var tcs = new TaskCompletionSource<I_User>();

        GetUserByID(id, user =>
        {
            tcs.SetResult(user);
        });

        return tcs.Task;
    }

    public void GetAllUsers(Action<List<I_User>> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetAllUsers(callback));
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


    public async Task<I_User.UserData> GetUserDataAsync(string id)
    {
        var tcs = new TaskCompletionSource<I_User.UserData>();

        GetUserByID(id, (user) =>
        {
            if (user != null && user.userData != null)
                tcs.SetResult(user.userData);
            else
                tcs.SetResult(null);
        });

        return await tcs.Task;
    }

    public void GetUserPersonalStat(string id, System.Action<I_User.PersonalStats> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.personalStats != null)
                callback?.Invoke(user.personalStats);
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

    public Task<List<I_User.Critteron>> GetUserCritteronsAsync()
    {
        var tcs = new TaskCompletionSource<List<I_User.Critteron>>();

        RequestUserInfo.Instance.GetUserCritterons(PlayerPrefs.GetString("UserID"), userCritteron =>
        {
            tcs.SetResult(userCritteron);
        });

        return tcs.Task;
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

    public async Task<I_User.Critteron> GetUserCritteronsByIDAsync(string userId, string critteronId)
    {
        var tcs = new TaskCompletionSource<I_User.Critteron>();

        GetUserByID(userId, (user) =>
        {
            if (user != null && user.critterons != null)
            {
                var critteron = user.critterons.Find(c => c.critteronID == critteronId);
                tcs.SetResult(critteron);
                return;
            }

            tcs.SetResult(null);
        });

        return await tcs.Task;
    }


    public void GetUserRoomsOwned(string id, System.Action<List<I_User.RoomOwned>> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.userData != null)
                callback?.Invoke(user.roomOwned);
            else
                callback?.Invoke(null);
        });
    }


    public void GetUserPendingFriend(string id, System.Action<List<I_User.PendingSocialStat>> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.pendingSocialStats != null)
                callback?.Invoke(user.pendingSocialStats);
            else
                callback?.Invoke(null);
        });
    }


    public void GetUserSentFriend(string id, System.Action<List<I_User.SentSocialStat>> callback)
    {
        GetUserByID(id, (user) =>
        {
            if (user != null && user.sentSocialStats != null)
                callback?.Invoke(user.sentSocialStats);
            else
                callback?.Invoke(null);
        });
    }



    public Task<List<string>> GetUserRoomsOwnedAsync()
    {
        var tcs = new TaskCompletionSource<List<string>>();

        RequestUserInfo.Instance.GetUserRoomsOwned(PlayerPrefs.GetString("UserID"), rooms =>
        {
            List<string> roomIDs = new List<string>();
            foreach (var room in rooms)
            {
                roomIDs.Add(room.roomID);
            }
            tcs.SetResult(roomIDs);
        });

        return tcs.Task;
    }

    /// <summary>
    /// Crea un nuevo usuario con el nombre de usuario y contraseña proporcionados
    /// </summary>
    /// <param name="nickname">Nombre de usuario</param>
    /// <param name="password">Contraseña</param>
    public void CreateNewUser(string nickname, string password, string mail)
    {
        StartCoroutine(ServerConnection.Instance.CreateNewUser(nickname, password, mail, (success) =>
        {
            if (success)
                Debug.Log("User created successfully!");
            else
                Debug.LogError("Failed to create user.");
        }));

    }

    public void ModifyUserRooms(string id, string newValue)
    {
        StartCoroutine(ServerConnection.Instance.ModifyUserField(id, "roomOwned", newValue));
    }


    public void ModifyUserData(string idUser, string nickname = null, int? level = null,
    int? experience = null, int? money = null, string currentCritteron = null)
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
                ["currentCritteron"] = currentCritteron ?? currentData.currentCritteron,
                ["lastClosedTime"] = currentData.lastClosedTime
            };


            StartCoroutine(ServerConnection.Instance.ModifyUserField(idUser, "userData", newValue));
        });
    }


    public void ModifyUserTime(string idUser)
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
                ["name"] = currentData.name,
                ["level"] = currentData.level,
                ["experience"] = currentData.experience,
                ["money"] = currentData.money,
                ["currentCritteron"] = currentData.currentCritteron,
                ["lastClosedTime"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
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
    float? currentLife = null, int? exp = null, int? stepAsPartner = null, int? usedAttacks = null,
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
                    exp = exp ?? 0,
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
            jsonObject["exp"] = critteron.exp;

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

    public async Task ModifyUserCritteronLifeTimeWithoutTimePass(string idUser)
    {
        int baseHeal = 10;
        int roomHealBonus = 5;

        var listRoom = await GetUserRoomsOwnedAsync();

        foreach (var room in listRoom)
        {
            var roomData = await RequestGameInfo.Instance.GetRoomByIDAsync(room);
            if (roomData.type == 1)
            {
                baseHeal += roomHealBonus;
            }
        }

        var critteronList = await GetUserCritteronsAsync();

        for (int i = 0; i < critteronList.Count; i++)
        {
            var critteron = await RequestGameInfo.Instance.GetCritteronByIDAsync(critteronList[i].critteronID);
            var critteronUser = await GetUserCritteronsByIDAsync(idUser, critteronList[i].critteronID);

            int newLife = (int)critteronUser.currentLife + (int)baseHeal;
            if (newLife > critteron.life + critteronUser.level)
                newLife = critteron.life + critteronUser.level;

            ModifyUserCritteron(idUser, critteronList[i].critteronID, currentLife: newLife);

        }

        ModifyUserTime(PlayerPrefs.GetString("UserID"));

    }


    public async Task ModifyUserCritteronLifeTime(string idUser)
    {
        float addLife = 20;

        var listRoom = await GetUserRoomsOwnedAsync();

        foreach (var room in listRoom)
        {
            var roomData = await RequestGameInfo.Instance.GetRoomByIDAsync(room);
            if (roomData.type == 1)
            {
                addLife += roomData.percent;
            }
        }

        int cureTime = RequestGameInfo.Instance.GetCureTime();
        var userData = await GetUserDataAsync(idUser);

        long dif = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - userData.lastClosedTime;
        Debug.Log(dif);

        float lifeHealth = (dif / cureTime) * addLife;


        var critteronList = await GetUserCritteronsAsync();

        for (int i = 0; i < critteronList.Count; i++)
        {
            var critteron = await RequestGameInfo.Instance.GetCritteronByIDAsync(critteronList[i].critteronID);
            var critteronUser = await GetUserCritteronsByIDAsync(idUser, critteronList[i].critteronID);

            int newLife = (int)critteronUser.currentLife + (int)lifeHealth;
            if (newLife > critteron.life + critteronUser.level)
                newLife = critteron.life + critteronUser.level;

            ModifyUserCritteron(idUser, critteronList[i].critteronID, currentLife: newLife);

        }

        ModifyUserTime(PlayerPrefs.GetString("UserID"));

    }

    public void GetExtraRoomType(string idUser, int type, Action<float> callback)
    {
        float extra = 0;

        GetUserRoomsOwned(idUser, list =>
        {
            foreach (var room in list)
            {
                RequestGameInfo.Instance.GetRoomByID(room.roomID, r =>
                {
                    if (r.type == type)
                    {
                        extra += r.percent;
                    }

                    callback?.Invoke(extra);
                    return;

                });
            }
        });
    }

    public async Task<float> GetExtraRoomTypeAsync(string idUser, int type)
    {
        float extra = 0;
        List<string> list = await GetUserRoomsOwnedAsync();

        foreach (var room in list)
        {
            var r = await RequestGameInfo.Instance.GetRoomByIDAsync(room);
            if (r.type == type)
            {
                extra += r.percent;
            }
        }

        return extra;
    }

    public void GetTopThreeGlobal(Action<List<String>> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetUserTop(callback));
    }

    public Task<List<String>> GetTopThreeGlobalAsync()
    {
        var tcs = new TaskCompletionSource<List<String>>();

        GetTopThreeGlobal(top =>
        {
            tcs.SetResult(top);
        });

        return tcs.Task;
    }

    public void GetTopThreeFriendsById(string userId, Action<List<string>> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetUserTopFriendsById(userId, callback));
    }

    public Task<List<string>> GetTopThreeFriendsByIdAsync(string userId)
    {
        var tcs = new TaskCompletionSource<List<string>>();

        GetTopThreeFriendsById(userId, top =>
        {
            tcs.SetResult(top);
        });

        return tcs.Task;
    }

}
