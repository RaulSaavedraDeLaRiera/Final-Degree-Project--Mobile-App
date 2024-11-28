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
    private string serverPath = "Assets/Scripts/Server/JAR/tfg-0.0.1-SNAPSHOT.jar";
    private Process serverProcess;

    private I_GameInfo gameInfo = null;
    private List<string> critteronIDs = new List<string>();
    private List<string> furnitureIDs = new List<string>();

    private I_UserInfo userInfo = null;
    private List<string> userIDs = new List<string>();

    private bool isServerReady = false;

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

    void Start()
    {
#if PLATFORM_ANDROID
        ConectionAndroid();
#endif

#if UNITY_EDITOR
        StartCoroutine("ConectionEditor");
#endif
    }

    void OnApplicationQuit()
    {
        StopServer();
    }
    void OnDisable()
    {
        StopServer();
    }

    void ConectionAndroid()
    {
    }

    /// <summary>
    /// Conectamos con el servidor y damos paso a la escena de juego una vez se haya realizado la coneccion
    /// </summary>
    private void ConectionEditor()
    {
        string jarPath;
        jarPath = Path.Combine(Application.dataPath, "..", serverPath);
        jarPath = Path.GetFullPath(jarPath);

        serverProcess = new Process();
        serverProcess.StartInfo.FileName = "java";
        serverProcess.StartInfo.Arguments = $"-jar \"{jarPath}\"";

        serverProcess.StartInfo.UseShellExecute = false;
        serverProcess.StartInfo.RedirectStandardOutput = true;
        serverProcess.StartInfo.RedirectStandardError = true;
        serverProcess.StartInfo.CreateNoWindow = true;

        serverProcess.OutputDataReceived += (sender, args) =>
        {
            UnityEngine.Debug.Log("Output: " + args.Data);
        };
        serverProcess.ErrorDataReceived += (sender, args) =>
        {
            UnityEngine.Debug.LogError("Error: " + args.Data);
        };

        try
        {
            serverProcess.Start();
            serverProcess.BeginOutputReadLine();
            serverProcess.BeginErrorReadLine();

            StartCoroutine(WaitForServerReady());
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Failed to start server: " + e.Message);
        }
    }

    /// <summary>
    /// Antes de inicializar el gameInfo, el UserInfo y cambiar de escena comprobamos que el server se haya conectado correctamente
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForServerReady()
    {
        float timeout = 30f;
        float elapsedTime = 0f;
        string checkUrl = "http://localhost:8080/connected";

        UnityEngine.Debug.Log("Checking if the server is ready...");

        while (!isServerReady && elapsedTime < timeout)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(checkUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success && request.downloadHandler.text.Contains("Server fully conected"))
                {
                    isServerReady = true;
                    break;
                }
            }

            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        if (isServerReady)
        {
            yield return StartCoroutine(GameInfoInit());
            yield return StartCoroutine(UserInfoInit());
            SceneManager.LoadScene("Login");
        }
    }

    /// <summary>
    /// Acabar con el proceso de coneccion con el servidor 
    /// </summary>
    private void StopServer()
    {
        if (serverProcess != null && !serverProcess.HasExited)
        {
            try
            {
                serverProcess.Kill();
                serverProcess.WaitForExit();
                UnityEngine.Debug.Log("Server stopped.");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("Failed to stop server: " + e.Message);
            }
        }
    }

    /// <summary>
    /// Guardamos todas las variables del juego asi como los id de las entidades
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameInfoInit()
    {
        string url = "http://localhost:8080/api/v1/gameinfo";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                gameInfo = JsonUtility.FromJson<I_GameInfo>(json);

                foreach (var critteron in gameInfo.critterons)
                    critteronIDs.Add(critteron.critteronID);

                foreach (var forniture in gameInfo.forniture)
                    furnitureIDs.Add(forniture.fornitureID);

                UnityEngine.Debug.Log("GameInfo init");
            }
        }
    }

    /// <summary>
    /// Guardamos todos los id de usuarios
    /// </summary>
    /// <returns></returns>
    private IEnumerator UserInfoInit()
    {
        string url = "http://localhost:8080/api/v1/userinfo";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                userInfo = JsonUtility.FromJson<I_UserInfo>(json);

                foreach (var user in userInfo.users)
                {
                    userIDs.Add(user.userID);
                }

                UnityEngine.Debug.Log("UserInfo init");
            }
        }
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
            I_Critteron i_critteron = null;

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    i_critteron = JsonUtility.FromJson<I_Critteron>(json);
                }
            }

            callback(i_critteron);
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
    public IEnumerator GetAllCritteronAsync(Action<List<I_Critteron>> callback)
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
            I_Furniture i_furniture = null;

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    i_furniture = JsonUtility.FromJson<I_Furniture>(json);
                }
            }

            callback(i_furniture);
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
    public IEnumerator GetUserByID(string id, System.Action<I_User> callback)
    {
        if (userIDs.Contains(id))
        {
            string url = $"http://localhost:8080/api/v1/user/{id}";
            I_User i_user = null;
          
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("token")}");
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    i_user = JsonUtility.FromJson<I_User>(json);
                    callback(i_user);
                }
            }
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
    public IEnumerator GetAllUsersAsync(Action<List<I_User>> callback)
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
    /// Modificamos una varible del usuario
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

        string jsonPayload = json.ToString();
        string token = PlayerPrefs.GetString("token");

        using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {token}");

            yield return request.SendWebRequest();
        }
    }

    /// <summary>
    /// Creacion de un nuevo usuario
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public IEnumerator CreateNewUser(string nickname, string password)
    {
        string url = $"http://localhost:8080/api/v1/newUser";

        var json = new JSONObject();
        json["password"] = password;

        var userData = new JSONObject();
        userData["name"] = nickname;
        json["userData"] = userData;

        string jsonSend = json.ToString();


        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonSend);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
        }
    }

    public IEnumerator LoginToken(string mail, string password, System.Action<string> callback)
    {
        string url = "http://localhost:8080/api/v1/login";

        // Crear JSON del cuerpo
        var json = new JSONObject();
        json["password"] = password;
        json["mail"] = mail;
        string jsonSend = json.ToString();

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Configurar el cuerpo de la solicitud
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonSend);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Enviar la solicitud
            yield return request.SendWebRequest();

            // Manejo de la respuesta
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Respuesta exitosa
                string responseText = request.downloadHandler.text;
                callback(responseText);
            }
            else
            {
                

                callback(""); // Llamar al callback con un valor vacío en caso de error
            }
        }
    }
}



