using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xasu;
using Xasu.HighLevel;

using System.Threading.Tasks;
using Unity.VisualScripting;
using TinCan;

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




    public static async void MessageWithCustomVerb(string actionId, string verbId, string verbDisplay, DateTime? timestamp = null)
    {
        if (!xasuReady)
        {
            Debug.Log("Xasu no está listo. No se puede enviar: " + actionId);
            return;
        }

        var verbMap = new LanguageMap();
        verbMap.Add("en-US", verbDisplay);

        var nameMap = new LanguageMap();
        nameMap.Add("en-US", actionId);

        var statement = new Statement
        {
            verb = new Verb
            {
                id = new Uri(verbId),
                display = verbMap
            },
            target = new Activity
            {
                id = $"https://mential.io/xapi/objects/{actionId}",
                definition = new ActivityDefinition
                {
                    type = new Uri("https://w3id.org/xapi/seriousgames/activity-types/game-object"),
                    name = nameMap
                }
            },
            timestamp = timestamp?.ToUniversalTime()
        };

        try
        {
            await XasuTracker.Instance.Enqueue(statement);
            Debug.Log($"Mensaje personalizado enviado: {verbDisplay} - {actionId}");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error enviando mensaje XASU personalizado: " + ex.Message);
        }
    }

}
