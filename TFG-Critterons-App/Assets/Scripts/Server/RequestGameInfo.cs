using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestGameInfo : MonoBehaviour
{
    private static RequestGameInfo _instance;

    public static RequestGameInfo Instance
    {
        get
        {
            if (_instance == null)
            {
                UnityEngine.Debug.LogError("RequestGameInfo instance is null");
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

    // <summary>
    /// Obtener un critteron por ID
    /// </summary>
    public void GetCritteronByID(string id, Action<I_Critteron> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetCritteronByID(id, callback));
    }

    /// <summary>
    /// Obtener todos los critterons
    /// </summary>
    public void GetAllCritteron(Action<List<I_Critteron>> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetAllCritteronAsync(callback));
    }

    /// <summary>
    /// Obtener un furniture por ID
    /// </summary>
    public void GetFurnitureByID(string id, Action<I_Furniture> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetFurnitureByID(id, callback));
    }

    /// <summary>
    /// Obtener todos los furnitures
    /// </summary>
    public void GetAllFurniture(Action<List<I_Furniture>> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetAllFurnitureAsync(callback));
    }
}
