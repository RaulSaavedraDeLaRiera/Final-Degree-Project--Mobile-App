
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public List<RoomInfo> Rooms => rooms;

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


    public HotelObject GetRandomHotelObject()
    {

        List<HotelObject> objects = new List<HotelObject>();
        objects.AddRange(healthObjects);
        objects.AddRange(levelObjects);
        objects.AddRange(decorationObjects);


        int randomStart = UnityEngine.Random.Range(0, objects.Count);

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[(randomStart + i) % objects.Count].CurrentUser == null)
            {
                return objects[(randomStart + i) % objects.Count];
            }
        }

        return null;
    }

    void InitialiceRooms()
    {

        //datos desde servidor de habitaciones disponibles
        List<string> roomData = new List<string>();
        roomData.Add("exampleRoom");
        roomData.Add("room2");

        //datos desde servidor de muebles
        List<string> data = new List<string>();
        data.Add("1");
        data.Add("2");

        foreach (var room in rooms)
        {
            room.InitialiceRoom(roomData, data, this);
        }
    }

    async void InitialiceCritterons()
    {
        var listUserCritterons = await GetUserCritteronsAsync();

        List<I_Critteron> data = new List<I_Critteron>();
        foreach (var cUser in listUserCritterons)
        {
            var critteron = await GetCritteronByIDAsync(cUser.critteronID);
            data.Add(critteron);
        }

        foreach (var critteron in data)
        {
            RoomInfo room = null;
            do
            {
                var r = rooms[UnityEngine.Random.Range(0, rooms.Count)];
                if (r.AvailableSpace)
                    room = r;

            } while (room == null);

            var hotelCritteron = Instantiate(critteronPrefab, room.EntryPoint.position, room.EntryPoint.rotation, critteronsRoot)
                .GetComponent<HotelCritteron>();

            hotelCritteron.InitialiceCritteron(critteron, room);
        }
    }



    private Task<List<I_User.Critteron>> GetUserCritteronsAsync()
    {
        var tcs = new TaskCompletionSource<List<I_User.Critteron>>();

        RequestUserInfo.Instance.GetUserCritterons(PlayerPrefs.GetString("UserID"), userCritteron =>
        {
            tcs.SetResult(userCritteron);
        });

        return tcs.Task;
    }

    private Task<I_Critteron> GetCritteronByIDAsync(string critteronID)
    {
        var tcs = new TaskCompletionSource<I_Critteron>();

        RequestGameInfo.Instance.GetCritteronByID(critteronID, critteron =>
        {
            tcs.SetResult(critteron);
        });

        return tcs.Task;
    }
}
