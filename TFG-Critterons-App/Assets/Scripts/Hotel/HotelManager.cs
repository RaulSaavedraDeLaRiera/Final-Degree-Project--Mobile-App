
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static I_UserInfo;

public class HotelManager : MonoBehaviour
{
    [SerializeField]
    Transform roomsRoot, critteronsRoot;
    [SerializeField]
    GameObject critteronPrefab;

    [SerializeField]
    Canvas canvas;
    [SerializeField]
    WaitingAnimation waitingAnimation;

    List<RoomInfo> rooms = new List<RoomInfo>();
    List<CritteronCombat> userCritterons;

    List<HotelObject> healthObjects = new List<HotelObject>(),
        levelObjects = new List<HotelObject>(),
        decorationObjects = new List<HotelObject>();

    public List<RoomInfo> Rooms => rooms;
    public List<int> roomPrices;
    I_User user;
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


        UpdateInfo();

        InitHotel();
    }



    async Task SetUser()
    {
        user = await RequestUserInfo.Instance.GetUserAsync(PlayerPrefs.GetString("UserID"));

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

    public HotelObject GetRandomHotelObject(RoomInfo roomSelected)
    {

        List<HotelObject> objects = new List<HotelObject>();
        objects.AddRange(healthObjects);
        objects.AddRange(levelObjects);
        objects.AddRange(decorationObjects);


        int randomStart = UnityEngine.Random.Range(0, objects.Count);

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[(randomStart + i) % objects.Count].Room == roomSelected && objects[(randomStart + i) % objects.Count].CurrentUser == null)
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


        AudioManager.m.PlaySound("upgrade");

        //guardar en servidor
        RequestUserInfo.Instance.ModifyUserData(PlayerPrefs.GetString("UserID"), money: money);
        RequestUserInfo.Instance.ModifyUserRooms(PlayerPrefs.GetString("UserID"), targetRoom.gameObject.name);
        RequestUserInfoSocial.Instance.ModifyPersonalStats(PlayerPrefs.GetString("UserID"), percentHotel: user.personalStats.percentHotel + 1);

        targetRoom.InitialiceRoom(this);

        //acciones visuales
        button.OutAnimation();
        GetComponent<HotelInput>().InputEnable = true;

        InfoCache.Reload();
        XasuControl.MessageWithCustomVerb(
            actionId: "Buy_Room_" + targetRoom.name,
            verbId: "http://adlnet.gov/expapi/verbs/interacted",
            verbDisplay: "interacted",
            timestamp: DateTime.UtcNow
        );
        UpdateInfo();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    async void InitHotel()
    {
        await InitialiceRooms();
        await InitialiceCritterons();

        if (waitingAnimation != null)
            waitingAnimation.Hide(1);
    }

    async Task InitialiceRooms()
    {
        List<I_Room> cachedRooms = InfoCache.GetCachedRooms();

        if (cachedRooms.Count == 0)
        {
            await InfoCache.LoadRoomsAsync();
            cachedRooms = InfoCache.GetCachedRooms();
        }

        roomPrices.Clear();
        foreach (var room in cachedRooms)
        {
            roomPrices.Add(room.price);
        }

        List<string> roomData = await RequestUserInfo.Instance.GetUserRoomsOwnedAsync();
        foreach (var room in rooms)
        {
            Debug.Log("numero de habitaciones compradas: " + roomData.Count);
            room.InitialiceRoom(roomData, this);
        }
    }


    async Task InitialiceCritterons()
    {
        var listUserCritterons = await RequestUserInfo.Instance.GetUserCritteronsAsync();

        List<I_Critteron> data = new List<I_Critteron>();
        foreach (var cUser in listUserCritterons)
        {
            var critteron = await RequestGameInfo.Instance.GetCritteronByIDAsync(cUser.critteronID);
            data.Add(critteron);
        }

        int startIndex = UnityEngine.Random.Range(0, data.Count);

        int critTotal = 0;

        for (int i = 0; i < data.Count; i++)
        {
            var critteron = data[(startIndex + i) % data.Count];

            // Buscar habitaciones válidas
            var availableRooms = rooms
                .Where(r => r.Bought && r.AvailableSpace())
                .ToList();

            if (availableRooms.Count == 0)
            {
                Debug.Log("mas criterrons que espacios en el hotel: criterrons asignados " + critTotal);
                // No hay habitaciones disponibles, se detiene el proceso
                break;
            }

            // Seleccionar una habitación aleatoria
            var room = availableRooms[UnityEngine.Random.Range(0, availableRooms.Count)];

            var hotelCritteron = Instantiate(critteronPrefab, room.EntryPoint.position, room.EntryPoint.rotation, critteronsRoot)
                .GetComponent<HotelCritteron>();

            hotelCritteron.InitialiceCritteron(critteron, room);
            room.AddCritteron(hotelCritteron);
            room.PlaceUsed();
            critTotal++;
        }
    }


    public void changeToPersonalStats()
    {

        //RequestUserInfo.Instance.ModifyUserCritteronLifeTime(PlayerPrefs.GetString("UserID"));
        SceneManager.LoadScene("UserInfo");
    }


    public void UpdateInfo()
    {
        Transform buttonTransform = canvas.transform.Find("Money");
        TextMeshProUGUI moneyText = buttonTransform.GetComponentInChildren<TextMeshProUGUI>();

        Transform nameBar = canvas.transform.Find("UserInfo");
        Transform nameText = nameBar.transform.Find("NameText");
        TextMeshProUGUI name = nameText.GetComponentInChildren<TextMeshProUGUI>();

        Transform levelText = nameBar.transform.Find("UserIcon");
        TextMeshProUGUI levelTextC = levelText.GetComponentInChildren<TextMeshProUGUI>();

        RequestUserInfo.Instance.GetUserByID(PlayerPrefs.GetString("UserID"), userData =>
        {
            user = userData;
            money = userData.userData.money;
            moneyText.text = money.ToString();

            name.text = userData.userData.name;
            levelTextC.text = userData.userData.level.ToString();
        });

    }

}
