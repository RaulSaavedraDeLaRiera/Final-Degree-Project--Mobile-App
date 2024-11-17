using System.Diagnostics;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using static System.Net.WebRequestMethods;

public class ServerConnection : MonoBehaviour
{
    private static ServerConnection _instance;
    private string serverPath = "Assets/Scripts/Server/JAR/tfg-0.0.1-SNAPSHOT.jar";
    private Process serverProcess;

    private I_GameInfo gameInfo;
    private List<string> critteronIDs = new List<string>();
    private List<string> furnitureIDs = new List<string>();

    private I_UserInfo userInfo;
    private List<string> userIDs = new List<string>();

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
        else
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
            UnityEngine.Debug.Log("Server started.");

            StartCoroutine(GameInfoInit());
            StartCoroutine(UserInfoInit());

            SceneManager.LoadScene("Hotel");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Failed to start server: " + e.Message);
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

                critteronIDs.Clear();
                furnitureIDs.Clear();

                foreach (var critteron in gameInfo.critterons)
                {
                    critteronIDs.Add(critteron.critteronID);
                }

                foreach (var forniture in gameInfo.forniture)
                {
                    furnitureIDs.Add(forniture.fornitureID);
                }
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

                userIDs.Clear();

                foreach (var user in userInfo.users)
                {
                    userIDs.Add(user.userID);
                }
            }
        }
    }


    /// <summary>
    /// Pedimos la informacion de un critteron a traves de su id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public I_Critteron GetCritteronByID(string id)
    {
        if (critteronIDs.Contains(id))
        {
            string url = $"http://localhost:8080/api/v1/critteron/{id}";
            I_Critteron i_critteron = null;

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    i_critteron = JsonUtility.FromJson<I_Critteron>(json);
                }
            }

            return i_critteron;
        }
        else
        {
            UnityEngine.Debug.LogError("Critteron not found");
            return null;
        }
    }

    /// <summary>
    /// Lista con todos los critterons de la base de datos
    /// </summary>
    /// <returns></returns>
    public List<I_Critteron> GetAllCritteron()
    {
        List<I_Critteron> i_critteronList = null;

        foreach (string id in critteronIDs)
            i_critteronList.Add(GetCritteronByID(id));

        return i_critteronList;
    }


    /// <summary>
    /// Pedimos la informacion de un furniture a traves de su id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public I_Furniture GetFurnitureByID(string id)
    {
        if (furnitureIDs.Contains(id))
        {
            string url = $"http://localhost:8080/api/v1/furniture/{id}";
            I_Furniture i_furniture = null;

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    i_furniture = JsonUtility.FromJson<I_Furniture>(json);
                }
            }

            return i_furniture;
        }
        else
        {
            UnityEngine.Debug.LogError("Furniture not found");
            return null;
        }
    }

    /// <summary>
    /// Lista con todos los furniture de la base de datos
    /// </summary>
    /// <returns></returns>
    public List<I_Furniture> GetAllFurnitures()
    {
        List<I_Furniture> i_furnitureList = null;

        foreach (string id in furnitureIDs)
            i_furnitureList.Add(GetFurnitureByID(id));

        return i_furnitureList;
    }


    public I_User GetUserByID(string id)
    {
        if (userIDs.Contains(id))
        {
            string url = $"http://localhost:8080/api/v1/user/{id}";
            I_User i_user = null;

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    i_user = JsonUtility.FromJson<I_User>(json);
                }
            }

            return i_user;
        }
        else
        {
            UnityEngine.Debug.LogError("User not found");
            return null;
        }
    }

    /// <summary>
    /// Lista con todos los users de la base de datos
    /// </summary>
    /// <returns></returns>
    public List<I_User> GetAllUsers()
    {
        List<I_User> i_userList = null;

        foreach (string id in userIDs)
            i_userList.Add(GetUserByID(id));

        return i_userList;
    }

    public IEnumerator ModifyUserField<T>(string userId, string fieldName, T newValue)
    {
        string url = $"http://localhost:8080/api/v1/user/{userId}";

        var payload = new
        {
            fieldName = fieldName,
            newValue = newValue
        };

        string jsonPayload = JsonUtility.ToJson(payload);

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonPayload))
        {
            request.method = "PATCH"; 
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
        }
    }
}

