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

    public I_Critteron GetCritteronByID(string ID)
    { 
        return ServerConnection.Instance.GetCritteronByID(ID);
    }

    public List<I_Critteron> GetAllCritteron()
    {
        return ServerConnection.Instance.GetAllCritteron();
    }

    public I_Furniture GetFurnitureByID(string ID)
    {
        return ServerConnection.Instance.GetFurnitureByID(ID);
    }

    public List<I_Furniture> GetAllFurniture()
    {
        return ServerConnection.Instance.GetAllFurnitures();
    }
}
