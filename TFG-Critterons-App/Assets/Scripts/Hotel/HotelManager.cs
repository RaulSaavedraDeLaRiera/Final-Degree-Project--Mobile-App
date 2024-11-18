
using System.Collections.Generic;
using UnityEngine;

public class HotelManager : MonoBehaviour
{
    [SerializeField]
    Transform roomsRoot, critteronsRoot;
    [SerializeField]
    GameObject critteronPrefab;

    List<RoomInfo> rooms = new List<RoomInfo>();
    List<CritteronCombat> userCritterons;

    List<HotelObject> healthObjects = new List<HotelObject>(),
        levelObjects = new List<HotelObject>(),
        decorationObjects = new List<HotelObject>();



    void Awake()
    {
        RoomInfo room;

        for (int i = 0; i < roomsRoot.childCount; i++)
        {
            room = roomsRoot.GetChild(i).GetComponent<RoomInfo>();

            if(room != null)
            rooms.Add(room);
        }
    }

    void Start()
    {
        InitialiceRooms();
        InitialiceCritterons();
    }

    public void AddObject(HotelObject hotelObject, HotelObjectType typeObject)
    {
        switch (typeObject)
        {
            case HotelObjectType.cureObject:
                healthObjects.Add(hotelObject);
                break;
            case HotelObjectType.levelObject:
                levelObjects.Add(hotelObject);
                break;
            case HotelObjectType.decorationObject:
                decorationObjects.Add(hotelObject);
                break;
        }
    }

    public HotelObject GetHotelObject(HotelObjectType typeObject)
    {

        List<HotelObject> objects;

        switch (typeObject)
        {
            case HotelObjectType.cureObject:
                objects = healthObjects;
                break;
            case HotelObjectType.levelObject:
                objects = levelObjects;
                break;
            default:
                objects = decorationObjects;
                break;
        }


        int randomStart = UnityEngine.Random.Range(0, objects.Count);

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[(randomStart+i)%objects.Count].CurrentUser == null)
            {
                return objects[(randomStart + i) % objects.Count];
            }
        }

        return null;
    }

    void InitialiceRooms()
    {

        //datos desde servidor
        List<string> data = new List<string>();
        data.Add("1");
        data.Add("2");
        data.Add("3");

        foreach (var room in rooms)
        {
            room.InitialiceRoom(data, this);
        }
    }

    void InitialiceCritterons()
    {
        //datos del servidor
        List<I_Critteron> data = new List<I_Critteron>();
        I_Critteron critteron1 = new I_Critteron("c1", "bird", "birdMesh",
            1, 10, 3, 2);
        data.Add(critteron1);


        foreach (var critteron in data)
        {
            RoomInfo room = null;
            //presuponemos que siempre habra heucos suficiente para los critterons
            do
            {
                var r = rooms[UnityEngine.Random.Range(0, rooms.Count)];
                if (r.AvailableSpace)
                    room = r;

            } while (room == null);

            var hotelCritteron = Instantiate<GameObject>(critteronPrefab,
                room.EntryPoint.position, room.EntryPoint.rotation, critteronsRoot)
                .GetComponent<HotelCritteron>();

            hotelCritteron.InitialiceCritteron(critteron, room);
        }
    }
}
