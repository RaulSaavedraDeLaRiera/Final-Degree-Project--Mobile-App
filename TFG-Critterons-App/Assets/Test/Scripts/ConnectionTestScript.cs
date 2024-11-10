using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class ConnectionTestScript : MonoBehaviour
{
    private string serverPath = "Assets/Test/Scripts/tfg-0.0.1-SNAPSHOT.jar";
    private string testPath = "Assets/Test/Scripts/helloworld.jar";

    [SerializeField]
    bool server = false;

    [SerializeField]
    GameInfoManager gameInfoManager;

    private GameInfo gameInfo;


    void Start()
    {
#if PLATFORM_ANDROID
        ConectionAndroid();
#endif

#if UNITY_EDITOR
        StartCoroutine("ConectionEditor");
#endif
    }

    // SendRequest();

    void ConectionAndroid()
    {
        using (AndroidJavaClass javaClass = new AndroidJavaClass("com.example.MyJavaClass"))
        {
            string message = javaClass.CallStatic<string>("getMessage");
            UnityEngine.Debug.Log("Mensaje desde Java: " + message);
        }
    }

    void ConectionEditor()
    {

        string jarPath;
        if (server)
            jarPath = Path.Combine(Application.dataPath, "..", serverPath);
        else
            jarPath = Path.Combine(Application.dataPath, "..", testPath);

        jarPath = Path.GetFullPath(jarPath);

        Process process = new Process();
        process.StartInfo.FileName = "java";
        process.StartInfo.Arguments = $"-jar \"{jarPath}\"";

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        //// Captura la salida de consola y errores
        process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log("Output: " + args.Data);
        process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError("Error: " + args.Data);

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            //si se activa bloquea unity process.WaitForExit();

            gameInfoManager.Init();

            UnityEngine.Debug.Log("Process exited with code: " + process.ExitCode);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Failed to start process: " + e.Message);
        }
    }

}



//    public void SendRequest()
//    {
//        // Crea una nueva instancia de Critteron
//        CritteronData critteron = new CritteronData
//        {
//            id = "30",
//            name = "antomon",
//            exp = 10,
//            life = 1
//        };


//        StartCoroutine(PostCritteron(critteron));
//    }

//    IEnumerator PostCritteron(CritteronData critteron)
//    {
//        // Convierte el objeto a JSON
//        string json = JsonUtility.ToJson(critteron);

//        // Solicitud POST
//        using (UnityWebRequest request = UnityWebRequest.PostWwwForm("http://localhost:8081/api/v1/critterons", json))
//        {
//            // Establece el tipo de contenido como JSON
//            request.SetRequestHeader("Content-Type", "application/json");

//            // Carga datos en pitici�n
//            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
//            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
//            request.downloadHandler = new DownloadHandlerBuffer();

//            // Env�a la solicitud y espera
//            yield return request.SendWebRequest();

//            // Logs de la respuesta
//            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
//            {
//                UnityEngine.Debug.LogError("Error: " + request.error);
//            }
//            else
//            {
//                UnityEngine.Debug.Log("Response: " + request.downloadHandler.text);
//            }
//        }
//    }
//}


//[System.Serializable]

//public class CritteronData
//{
//    public string id;
//    public string name;
//    public int exp;
//    public int life;
//}
