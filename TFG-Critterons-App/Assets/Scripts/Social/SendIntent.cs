using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SendIntent : MonoBehaviour
{
    AndroidJavaClass IntentClass;
    AndroidJavaObject sendIntent;

    AndroidJavaClass UnityPlayer;
    AndroidJavaObject currentActivity;

    bool IsInitialized = false;

    [SerializeField] GameObject sprite;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("initialized");

    }

    void Initialize()
    {
        IsInitialized = true;

        string className = "android.content.Intent";
        IntentClass = new AndroidJavaClass(className);

        //Intent sendIntent = new Intent();
        sendIntent = new AndroidJavaObject(className);

        UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //UnityPlayer = new AndroidJavaClass("com.DefaultCompany.EjemploMoviles");
        currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }


    /*
    public void SendTheIntent(string text)
    {
        if (sprite != null)
        {
            sprite.GetComponentInChildren<SpriteRenderer>().color = new Color(0, 0, 1, 1);
            Debug.Log("pulsadoooo");
        }

        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }

#if UNITY_ANDROID
        if (!IsInitialized)
        {
            Initialize(); // Initialize Android-related objects
        }

        //sendIntent.setAction(Intent.ACTION_SEND);
        sendIntent.Call<AndroidJavaObject>("setAction", IntentClass.GetStatic<string>("ACTION_SEND"));

        //sendIntent.putExtra(Intent.EXTRA_TEXT, textMessage);
        sendIntent.Call<AndroidJavaObject>("putExtra", IntentClass.GetStatic<string>("EXTRA_TEXT"), text);

        //sendIntent.setType("text/plain");
        sendIntent.Call<AndroidJavaObject>("setType", "text/plain");

        //startActivity(sendIntent);
        currentActivity.Call("startActivity", sendIntent);

#endif
    }
    */

    string GetData()
    {
        //poner codigo para obtener id
        return "";
    }

    public void SendTheIntet(string text)
    {
        //get data y se amplia o si asi vale
        var textSource = transform.GetComponentInChildren<TextMeshProUGUI>();

        if (Application.platform != RuntimePlatform.Android || textSource == null)
        {
            return;
        }

#if UNITY_ANDROID
        if (!IsInitialized)
        {
            Initialize(); // Initialize Android-related objects
        }

        //sendIntent.setAction(Intent.ACTION_SEND);
        sendIntent.Call<AndroidJavaObject>("setAction", IntentClass.GetStatic<string>("ACTION_SEND"));

        //sendIntent.putExtra(Intent.EXTRA_TEXT, textMessage);
        sendIntent.Call<AndroidJavaObject>("putExtra", IntentClass.GetStatic<string>("EXTRA_TEXT"), (text + textSource.text));

        //sendIntent.setType("text/plain");
        sendIntent.Call<AndroidJavaObject>("setType", "text/plain");

        //startActivity(sendIntent);
        currentActivity.Call("startActivity", sendIntent);

#endif
    }

    public void ShareXasuTraces()
    {
#if UNITY_ANDROID

        string sourcePath = System.IO.Path.Combine(Application.persistentDataPath, "traces.log");

        if (!System.IO.File.Exists(sourcePath))
        {
            Debug.LogWarning("Archivo no encontrado: " + sourcePath);
            return;
        }

        try
        {
            AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment");
            AndroidJavaObject downloadsDir = environment.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", environment.GetStatic<string>("DIRECTORY_DOWNLOADS"));

            string targetPath = System.IO.Path.Combine(downloadsDir.Call<string>("getAbsolutePath"), "traces.log");

            System.IO.File.Copy(sourcePath, targetPath, overwrite: true);

            Debug.Log($"Archivo copiado a: {targetPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error al copiar archivo a Downloads: " + ex.Message);
        }
#endif
    }

}




