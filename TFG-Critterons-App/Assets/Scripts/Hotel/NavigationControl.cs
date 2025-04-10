using UnityEngine;
using UnityEngine.AI;

public class NavigationControl : MonoBehaviour
{
    static NavigationControl instance;

    HotelManager hotelManager;

    static int maxTriesToGetPosition = 50;

    static bool globalMove = true;

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

    public static HotelObject GetTarget(RoomInfo specificRoom = null)
    {
        if (specificRoom == null)
            return instance.hotelManager.GetRandomHotelObject();
        else
            return instance.hotelManager.GetRandomHotelObject(specificRoom);

    }

    public static Transform GetNextRoutePoint(HotelCritteron critteron)
    {
        if (critteron.Target.Room == critteron.CurrentRoom)
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
        Vector3 roomPos; Vector2 roomSize;

        if (!globalMove)
        {
            roomPos = critteron.CurrentRoom.transform.position;
            roomSize = critteron.CurrentRoom.Size;
        }
        else
        {
            var room =
                instance.hotelManager.Rooms[Random.Range(0, instance.hotelManager.Rooms.Count)];
            roomPos = room.transform.position;
            roomSize = room.Size;
        }


        NavMeshHit hit;

        for (int i = 0; i < maxTriesToGetPosition; i++)
        {
            if (NavMesh.SamplePosition(
           new Vector3(roomPos.x + Random.Range(-roomSize.x, roomSize.x), roomPos.y, roomPos.z + Random.Range(-roomSize.y, roomSize.y)),
           out hit, Mathf.Infinity, NavMesh.AllAreas))
            {
                return hit.position; // Devuelve la posición válida en el NavMesh
            }
        }

        return critteron.transform.position;

    }
}
