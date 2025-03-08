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
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;





public class ServerConnection : MonoBehaviour
{
    private static ServerConnection _instance;

    private I_GameInfo gameInfo = null;
    private List<string> critteronIDs = new List<string>();
    private List<string> roomIDs = new List<string>();

    private I_UserInfo userInfo = null;
    private List<string> userIDs = new List<string>();
    private float timeOut = 15f;


    private ServerConfig config;
    private string configPath;

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
            LoadConfig();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void LoadConfig()
    {
        configPath = Path.Combine(Application.persistentDataPath, "server_config.json");

        if (System.IO.File.Exists(configPath))
        {
            UnityEngine.Debug.Log("Archivo de configuración encontrado");
            UnityEngine.Debug.Log(Application.persistentDataPath);
            string json = System.IO.File.ReadAllText(configPath);
            config = JsonUtility.FromJson<ServerConfig>(json);
        }
        else
        {
            UnityEngine.Debug.Log("Archivo de configuración no encontrado");

            config = new ServerConfig { baseURL = "http://192.168.1.132", port = "8080", apiVersion = "v1/" };
            System.IO.File.WriteAllText(configPath, JsonUtility.ToJson(config, true));
        }

        CheckLocalRoute();
    }

    private string GetFullURL(string endpoint)
    {
        string fullURL = config.baseURL;

        // agregamos puerto si existe
        if (!string.IsNullOrEmpty(config.port))
            fullURL += $":{config.port}";

        return $"{fullURL}/api/{config.apiVersion}{endpoint}";
    }

    void CheckLocalRoute()
    {
        string url = GetFullURL("login");
        string expectedUrl = "http://192.168.1.132:8080/api/v1/login";

        UnityEngine.Debug.Log("Ruta generada: " + url);
        UnityEngine.Debug.Log("Ruta esperada: " + expectedUrl);

        // Corregimos la comparación
        bool sonIguales = url == expectedUrl;
        UnityEngine.Debug.Log("¿Las rutas son iguales? " + sonIguales);

    }

    public IEnumerator LoginToken(string mail, string password, Action<string> callback)
    {
        // string url = $"http://localhost:8080/api/v1/login";
        string url = GetFullURL("login");

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

    public IEnumerator GetIDUser(string mail, string password, Action<string> callback)
    {
        //string url = $"http://localhost:8080/api/v1/id";
        string url = GetFullURL("id");

        var jsonPayload = new JSONObject();
        jsonPayload["mail"] = mail;
        jsonPayload["password"] = password;

        yield return StartCoroutine(SendRequest(url, "GET", jsonPayload,
            (response) =>
            {
                UnityEngine.Debug.Log("Getting id successfully");
                callback(response);
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"ID error: {error}");
                callback("");
            }
        ));
    }

    public IEnumerator CreateNewUser(string nickname, string password, string mail, Action<bool> callback)
    {
        //string url = $"http://localhost:8080/api/v1/newUser";
        string url = GetFullURL("newUser");

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

    public async Task GameInfoInitAsync()
    {
        var tcs = new TaskCompletionSource<bool>();

        StartCoroutine(GameInfoInitCoroutine(
            () => tcs.SetResult(true),
            (error) => tcs.SetException(new Exception($"Error fetching game info: {error}"))
        ));

        await tcs.Task;
    }

    private IEnumerator GameInfoInitCoroutine(Action onSuccess, Action<string> onError)
    {
        //string url = "http://localhost:8080/api/v1/gameinfo";
        string url = GetFullURL("gameinfo");

        yield return StartCoroutine(SendRequest(url, "GET", null,
            (response) =>
            {
                // Procesamos la respuesta exitosa
                gameInfo = JsonUtility.FromJson<I_GameInfo>(response);

                foreach (var critteron in gameInfo.critterons)
                    critteronIDs.Add(critteron.critteronID);

                foreach (var room in gameInfo.rooms)
                    roomIDs.Add(room.roomID);


                UnityEngine.Debug.Log("GameInfo init");
                onSuccess?.Invoke();
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Error fetching game info: {error}");
                onError?.Invoke(error);
            }
        ));
    }

    public async Task UserInfoInitAsync()
    {
        var tcs = new TaskCompletionSource<bool>();

        StartCoroutine(UserInfoInitCoroutine(
            () => tcs.SetResult(true),
            (error) => tcs.SetException(new Exception($"Error fetching user info: {error}"))
        ));

        await tcs.Task;
    }

    private IEnumerator UserInfoInitCoroutine(Action onSuccess, Action<string> onError)
    {
        //string url = "http://localhost:8080/api/v1/userinfo";
        string url = GetFullURL("userinfo");

        yield return StartCoroutine(SendRequest(url, "GET", null,
            (response) =>
            {
                // Procesamos la respuesta exitosa
                userInfo = JsonUtility.FromJson<I_UserInfo>(response);

                foreach (var user in userInfo.users)
                    userIDs.Add(user.userID);

                UnityEngine.Debug.Log("UserInfo init");
                onSuccess?.Invoke();
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Error fetching user info: {error}");
                onError?.Invoke(error);
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

        //string url = $"http://localhost:8080/api/v1/critteron/{id}";
        string url = GetFullURL($"critteron/{id}");

        yield return StartCoroutine(SendRequest(url, "GET", null,
            (response) =>
            {
                I_Critteron i_critteron = JsonUtility.FromJson<I_Critteron>(response);
                callback(i_critteron);
            },
            (error) =>
            {

                callback(null);
            }
        ));

    }

    public IEnumerator GetUserTop(Action<List<string>> callback)
    {
        List<string> list = new List<string>();
        //string url = $"http://localhost:8080/api/v1/userTop";
        string url = GetFullURL("userTop");

        yield return StartCoroutine(SendRequest(url, "GET", null,
            (response) =>
            {
                try
                {
                    List<string> list = JsonHelper.FromJson<string>(response);
                    callback(list);
                }
                catch (Exception e)
                {
                    callback(null);
                }
            },
            (error) =>
            {
                callback(null);
            }
        ));
    }

    public IEnumerator GetUserTopFriendsById(string userId, Action<List<string>> callback)
    {
        List<string> list = new List<string>();
        //string url = $"http://localhost:8080/api/v1/userTopFriends/{userId}";
        string url = GetFullURL($"userTopFriends/{userId}");

        yield return StartCoroutine(SendRequest(url, "GET", null,
            (response) =>
            {
                try
                {
                    list = JsonHelper.FromJson<string>(response);
                    callback(list);
                }
                catch (Exception e)
                {
                    callback(null);
                }
            },
            (error) =>
            {
                callback(null);
            }
        ));
    }


    public I_GameInfo GetGameInfo()
    {
        return gameInfo;
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
    /// Pedimos la información de un room a través de su ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetRoomByID(string id, Action<I_Room> callback)
    {

        //string url = $"http://localhost:8080/api/v1/room/{id}";
        string url = GetFullURL($"room/{id}");

        yield return StartCoroutine(SendRequest(url, "GET", null,
            (response) =>
            {
                I_Room i_Room = JsonUtility.FromJson<I_Room>(response);
                callback(i_Room);
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Error fetching room by ID {id}: {error}");
                callback(null);
            }
        ));

    }

    /// <summary>
    /// Lista con todos los room de la base de datos
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetAllRooms(Action<List<I_Room>> callback)
    {
        List<I_Room> i_roomsList = new List<I_Room>();

        foreach (string id in roomIDs)
        {
            yield return StartCoroutine(GetRoomByID(id, (room) =>
            {
                if (room != null)
                {
                    i_roomsList.Add(room);
                }
            }));
        }

        callback(i_roomsList);
    }

    /// <summary>
    /// Obtenemos a un usuario a traves de su id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetUserByID(string id, Action<I_User> callback)
    {
        //string url = $"http://localhost:8080/api/v1/user/{id}";
        string url = GetFullURL($"user/{id}");

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
        //string url = $"http://localhost:8080/api/v1/user/{userId}";
        string url = GetFullURL($"user/{userId}");

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

    public IEnumerator RemoveUserFriend(string userId, JSONObject friendID)
    {
        //string url = $"http://localhost:8080/api/v1/user/removeFriend/{userId}";
        string url = GetFullURL($"user/removeFriend/{userId}");

        yield return StartCoroutine(SendRequest(url, "DELETE", friendID,
            (response) =>
            {
                UnityEngine.Debug.Log("User friend deleted successfully");
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Error deleting user friend: {error}");
            }
        ));
    }

    public IEnumerator AddPendingFriend<T>(string userId, string fieldName, T newValue)
    {
        //string url = $"http://localhost:8080/api/v1/userPending/{userId}";
        string url = GetFullURL($"userPending/{userId}");

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
                UnityEngine.Debug.Log("User pending friend field modified successfully");
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Error modifying user pending friend field: {error}");
            }
        ));
    }

    public IEnumerator RemoveUserFriendPending(string userId, JSONObject friendID)
    {
        // string url = $"http://localhost:8080/api/v1/user/removeFriendPending/{userId}";
        string url = GetFullURL($"user/removeFriendPending/{userId}");

        yield return StartCoroutine(SendRequest(url, "DELETE", friendID,
            (response) =>
            {
                UnityEngine.Debug.Log("User pending friend deleted successfully");
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Error deleting pending user friend: {error}");
            }
        ));
    }

    public IEnumerator AddSentFriend<T>(string userId, string fieldName, T newValue)
    {
        //string url = $"http://localhost:8080/api/v1/userSent/{userId}";
        string url = GetFullURL($"userSent/{userId}");

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
                UnityEngine.Debug.Log("User sent friend field modified successfully");
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Error modifying user sent friend field: {error}");
            }
        ));
    }

    public IEnumerator RemoveUserFriendSent(string userId, JSONObject friendID)
    {
        //string url = $"http://localhost:8080/api/v1/user/removeFriendSent/{userId}";
        string url = GetFullURL($"user/removeFriendSent/{userId}");

        yield return StartCoroutine(SendRequest(url, "DELETE", friendID,
            (response) =>
            {
                UnityEngine.Debug.Log("User sent friend deleted successfully");
            },
            (error) =>
            {
                UnityEngine.Debug.LogError($"Error deleting sent user friend: {error}");
            }
        ));
    }

}


public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        string newJson = "{\"items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.items;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
    }
}

[Serializable]
public class ServerConfig
{
    public string baseURL;
    public string port;
    public string apiVersion;
}
