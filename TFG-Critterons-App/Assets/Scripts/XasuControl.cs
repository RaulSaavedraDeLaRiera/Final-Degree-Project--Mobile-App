using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xasu;
using Xasu.HighLevel;

using System.Threading.Tasks;
using Unity.VisualScripting;

public class XasuControl : MonoBehaviour
{
    static bool xasuReady = false;
    private void Start()
    {
        InitXasu();
    }

    private void OnDestroy()
    {
        GameObjectTracker.Instance.Interacted("LOGOUT");
        StopXasu();
    }

    public static async Task InitXasu()
    {
        try
        {
            await Xasu.XasuTracker.Instance.Init();
            Debug.Log("Xasu inicializado");
            xasuReady = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error al inicializar Xasu: " + ex.Message);
            xasuReady = false;
        }



    }

    public static void Message(string action)
    {
        if (!xasuReady)
        {
            Debug.Log("Xasu not ready, cant send: " + action);
            return;
        }

        Debug.Log("Xasu new interact: " + action);
        GameObjectTracker.Instance.Interacted(action);

    }

    private void OnApplicationQuit()
    {
        StopXasu();
    }

   /*
    private void OnApplicationPause(bool pause)
    {

        return;

        if (pause)
        {
            StopXasu();
        }
        else
            InitXasu();


    }
   */

    async static Task StopXasu()
    {
        xasuReady = false;

        var progress = new Progress<float>();
        progress.ProgressChanged += (_, p) =>
        {
            Debug.Log("Finalization progress: " + p);
        };
        await Xasu.XasuTracker.Instance.Finalize(progress);
        Debug.Log("Tracker finalized, game is now ready to close...");
    }
}
