using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationControl : MonoBehaviour
{
    static NavigationControl instance;

    HotelManager hotelManager;

    static int maxTriesToGetPosition = 50;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }


    private void Start()
    {
        hotelManager = GetComponent<HotelManager>();
    }

    public static HotelObject GetTarget(HotelObjectType typeObject)
    {
        return instance.hotelManager.GetHotelObject(typeObject);
    }

    public static Transform GetNextRoutePoint(HotelCritteron critteron)
    {
        if(critteron.Target.Room == critteron.CurrentRoom)
        {
            return critteron.Target.transform;
        }
        else
        {
            return critteron.CurrentRoom.EntryPoint;
        }
    }
    public static Vector3 GetRandomPoint(HotelCritteron critteron)
    {
        var roomPos = critteron.CurrentRoom.transform.position;
        var roomSize = critteron.CurrentRoom.Size;

        NavMeshHit hit;

        for (int i = 0; i < maxTriesToGetPosition; i++)
        {
            if (NavMesh.SamplePosition(
           new Vector3(roomPos.x + Random.Range(-roomSize.Item1, roomSize.Item1), roomPos.y, roomPos.z + Random.Range(-roomSize.Item2, roomSize.Item2)),
           out hit, Mathf.Infinity, NavMesh.AllAreas))
            {
                return hit.position; // Devuelve la posición válida en el NavMesh
            }
        }

        return critteron.transform.position;
       
    }
}
