
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class HotelManager : MonoBehaviour
{
    [SerializeField]
    Transform roomsRoot, critteronsRoot;
    [SerializeField]
    GameObject critteronPrefab;

    [SerializeField]
    Canvas canvas;

    List<RoomInfo> rooms = new List<RoomInfo>();
    List<CritteronCombat> userCritterons;

    List<HotelObject> healthObjects = new List<HotelObject>(),
        levelObjects = new List<HotelObject>(),
        decorationObjects = new List<HotelObject>();

    public List<RoomInfo> Rooms => rooms;
    public List<int> roomPrices;

    int money;

    void Awake()
    {
        RoomInfo room;

        for (int i = 0; i < roomsRoot.childCount; i++)
        {
            room = roomsRoot.GetChild(i).GetComponent<RoomInfo>();

            if (room != null)
                rooms.Add(room);
        }
    }

    void Start()
    {

        Transform buttonTransform = canvas.transform.Find("Money");
        TextMeshProUGUI moneyText = buttonTransform.GetComponentInChildren<TextMeshProUGUI>();

        RequestUserInfo.Instance.GetUserData(PlayerPrefs.GetString("UserID"), userData =>
        {
            money = userData.money;
            moneyText.text = money.ToString();
        });

        InitialiceRooms();
        // InitialiceCritterons();
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
            if (objects[(randomStart + i) % objects.Count].CurrentUser == null)
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


    public void TryBuyRoom(RoomInfo targetRoom, ButtonActions button)
    {

        if (targetRoom.Bought)
            return;

        int index = -1;

        for (int i = 0; i < rooms.Count; i++)
            if (rooms[i] == targetRoom)
            {
                index = i;
                break;
            }

        if (money < roomPrices[index])
            return;

        money -= roomPrices[index];

        //guardar en servidor

        targetRoom.InitialiceRoom(this);

        //acciones visuales
        button.OutAnimation();
        GetComponent<HotelInput>().InputEnable = true;

    }

    async void InitialiceRooms()
    {
        var roomsFromServer = await RequestGameInfo.Instance.GetAllRoomsAsync();
        foreach (var room in roomsFromServer)
        {
            roomPrices.Add(room.price);
        }

        List<string> roomData = await RequestUserInfo.Instance.GetUserRoomsOwnedAsync();

        foreach (var room in rooms)
        {
            room.InitialiceRoom(roomData, this);
        }
    }


    async void InitialiceCritterons()
    {
        var listUserCritterons = await RequestUserInfo.Instance.GetUserCritteronsAsync();

        List<I_Critteron> data = new List<I_Critteron>();
        foreach (var cUser in listUserCritterons)
        {
            var critteron = await RequestGameInfo.Instance.GetCritteronByIDAsync(cUser.critteronID);
            data.Add(critteron);
        }

        foreach (var critteron in data)
        {
            RoomInfo room = null;
            do
            {
                var r = rooms[UnityEngine.Random.Range(0, rooms.Count)];
                if (r.Bought)// && r.AvailableSpace)
                    room = r;

            } while (room == null);

            var hotelCritteron = Instantiate(critteronPrefab, room.EntryPoint.position, room.EntryPoint.rotation, critteronsRoot)
                .GetComponent<HotelCritteron>();

            hotelCritteron.InitialiceCritteron(critteron, room);
            room.AddCritteron(hotelCritteron);
        }
    }



   
}
