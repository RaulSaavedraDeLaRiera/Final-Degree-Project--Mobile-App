using System.Diagnostics;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using static System.Net.WebRequestMethods;
using System.Text;
using SimpleJSON;
using static I_UserInfo;
using System;

public class ServerConnection : MonoBehaviour
{
    private static ServerConnection _instance;

    private I_GameInfo gameInfo = null;
    private List<string> critteronIDs = new List<string>();
    private List<string> furnitureIDs = new List<string>();

    private I_UserInfo userInfo = null;
    private List<string> userIDs = new List<string>();
    private float timeOut = 15f;

    public static ServerConnection Instance
    {
        get
        {
            if (_instance == null)
            {
                UnityEngine.Debug.LogError("ServerConnection instance is null");
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator LoginToken(string mail, string password, Action<string> callback)
    {
        string url = $"http://localhost:8080/api/v1/login";

        var jsonPayload = new JSONObject();
        jsonPayload["mail"] = mail;
        jsonPayload["password"] = password;

        yield return StartCoroutine(SendRequest(url, "GET", jsonPayload,
            (response) =>
            {
                UnityEngine.Debug.Log("Login successful");
                callback(response);
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Login error: {error}");
                callback("");
            }
        ));
    }

    public IEnumerator CreateNewUser(string nickname, string password, string mail, Action<bool> callback)
    {
        string url = $"http://localhost:8080/api/v1/newUser";

        var jsonPayload = new JSONObject();
        jsonPayload["password"] = password;
        jsonPayload["mail"] = mail;

        var userData = new JSONObject();
        userData["name"] = nickname;
        jsonPayload["userData"] = userData;

        yield return StartCoroutine(SendRequest(url, "POST", jsonPayload,
            (response) =>
            {
                UnityEngine.Debug.Log("User created successfully");
                callback(true);
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Create user error: {error}");
                callback(false);
            }
        ));
    }

    private IEnumerator SendRequest(string url, string method, JSONObject jsonPayload, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (jsonPayload != null)
            {
                string jsonSend = jsonPayload.ToString();
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonSend);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            request.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("token")}");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            float elapsedTime = 0f;
            bool requestCompleted = false;

            request.SendWebRequest();

            while (!request.isDone)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= timeOut)
                {
                    onError?.Invoke("Timeout");
                    yield break;
                }
                yield return null;
            }

            if (request.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(request.downloadHandler.text);
            else
                onError?.Invoke(request.error ?? "Unknown error occurred.");
        }
    }

    /// <summary>
    /// Guardamos todas las variables del juego asi como los id de las entidades
    /// </summary>
    /// <returns></returns>
    public IEnumerator GameInfoInit()
    {
        string url = "http://localhost:8080/api/v1/gameinfo";

        yield return StartCoroutine(SendRequest(url, "GET", null,
            (response) =>
            {
                // Procesamos la respuesta exitosa
                gameInfo = JsonUtility.FromJson<I_GameInfo>(response);

                foreach (var critteron in gameInfo.critterons)
                    critteronIDs.Add(critteron.critteronID);

                foreach (var forniture in gameInfo.forniture)
                    furnitureIDs.Add(forniture.fornitureID);

                UnityEngine.Debug.Log("GameInfo init");
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Error fetching game info: {error}");
            }
        ));
    }

    /// <summary>
    /// Guardamos todos los id de usuarios
    /// </summary>
    /// <returns></returns>
    public IEnumerator UserInfoInit()
    {
        string url = "http://localhost:8080/api/v1/userinfo";

        yield return StartCoroutine(SendRequest(url, "GET", null,
            (response) =>
            {
                // Procesamos la respuesta exitosa
                userInfo = JsonUtility.FromJson<I_UserInfo>(response);

                foreach (var user in userInfo.users)
                    userIDs.Add(user.userID);

                UnityEngine.Debug.Log("UserInfo init");
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Error fetching user info: {error}");
            }
        ));
    }

    /// <summary>
    /// Pedimos la información de un critteron a través de su ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetCritteronByID(string id, Action<I_Critteron> callback)
    {
        if (critteronIDs.Contains(id))
        {
            string url = $"http://localhost:8080/api/v1/critteron/{id}";

            yield return StartCoroutine(SendRequest(url, "GET", null,
                (response) =>
                {
                    I_Critteron i_critteron = JsonUtility.FromJson<I_Critteron>(response);
                    callback(i_critteron);
                },
                (error) =>
                {
                    UnityEngine.Debug.LogError($"Error fetching critteron by ID {id}: {error}");
                    callback(null);
                }
            ));
        }
        else
        {
            UnityEngine.Debug.LogError("Critteron not found");
            callback(null);
        }
    }

    /// <summary>
    /// Lista con todos los critterons de la base de datos
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetAllCritteron(Action<List<I_Critteron>> callback)
    {
        List<I_Critteron> i_critteronList = new List<I_Critteron>();
        foreach (string id in critteronIDs)
        {
            yield return StartCoroutine(GetCritteronByID(id, (critteron) =>
            {
                if (critteron != null)
                {
                    i_critteronList.Add(critteron);
                }
            }));
        }
        callback(i_critteronList);
    }

    /// <summary>
    /// Pedimos la información de un furniture a través de su ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetFurnitureByID(string id, Action<I_Furniture> callback)
    {
        if (furnitureIDs.Contains(id))
        {
            string url = $"http://localhost:8080/api/v1/furniture/{id}";

            yield return StartCoroutine(SendRequest(url, "GET", null,
                (response) =>
                {
                    I_Furniture i_furniture = JsonUtility.FromJson<I_Furniture>(response);
                    callback(i_furniture);
                },
                (error) =>
                {
                    UnityEngine.Debug.LogError($"Error fetching furniture by ID {id}: {error}");
                    callback(null);
                }
            ));
        }
        else
        {
            UnityEngine.Debug.LogError("Furniture not found");
            callback(null);
        }
    }

    /// <summary>
    /// Lista con todos los furnitures de la base de datos
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetAllFurnitureAsync(Action<List<I_Furniture>> callback)
    {
        List<I_Furniture> i_furnitureList = new List<I_Furniture>();

        foreach (string id in furnitureIDs)
        {
            yield return StartCoroutine(GetFurnitureByID(id, (furniture) =>
            {
                if (furniture != null)
                {
                    i_furnitureList.Add(furniture);
                }
            }));
        }

        callback(i_furnitureList);
    }

    /// <summary>
    /// Obtenemos a un usuario a traves de su id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetUserByID(string id, Action<I_User> callback)
    {
        if (userIDs.Contains(id))
        {
            string url = $"http://localhost:8080/api/v1/user/{id}";
            string token = PlayerPrefs.GetString("token");

            yield return StartCoroutine(SendRequest(url, "GET", null,
                (response) =>
                {
                    I_User i_user = JsonUtility.FromJson<I_User>(response);
                    callback(i_user);
                },
                (error) =>
                {
                    UnityEngine.Debug.LogError($"Error fetching user by ID {id}: {error}");
                    callback(null);
                }
            ));
        }
        else
        {
            UnityEngine.Debug.LogError("User not found");
            callback(null);
        }
    }

    /// <summary>
    /// Lista con todos los users de la base de datos
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetAllUsers(Action<List<I_User>> callback)
    {
        List<I_User> i_userList = new List<I_User>();

        foreach (string id in userIDs)
        {
            yield return StartCoroutine(GetUserByID(id, (user) =>
            {
                if (user != null)
                {
                    i_userList.Add(user);
                }
            }));
        }

        callback(i_userList);
    }

    /// <summary>
    /// Modificamos una variable del usuario
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="userId"></param>
    /// <param name="fieldName"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    public IEnumerator ModifyUserField<T>(string userId, string fieldName, T newValue)
    {
        string url = $"http://localhost:8080/api/v1/user/{userId}";

        var json = new JSONObject();
        json["fieldName"] = fieldName;

        if (newValue is string || newValue is int || newValue is float)
            json["newValue"] = new JSONString(newValue.ToString());
        else if (newValue is JSONObject jsonValue)
            json["newValue"] = jsonValue;
        else
            json["newValue"] = JSON.Parse(JsonUtility.ToJson(newValue));

        yield return StartCoroutine(SendRequest(url, "PATCH", json,
            (response) =>
            {
                UnityEngine.Debug.Log("User field modified successfully");
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Error modifying user field: {error}");
            }
        ));
    }
}
