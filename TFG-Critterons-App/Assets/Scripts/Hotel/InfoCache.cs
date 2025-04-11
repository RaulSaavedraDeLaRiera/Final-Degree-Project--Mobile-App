using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InfoCache : MonoBehaviour
{
    private static InfoCache instance;
    private static List<I_Room> cachedRooms = new List<I_Room>();
    private static List<float> roomPrices = new List<float>();
    private static bool isDataLoaded = false;

    private static List<I_Mark> cachedMarks = new List<I_Mark>();
    private static bool areMarksLoaded = false;

    private bool isRunningLifeUpdater = false;
    private static int time;

    public static I_GameInfo gameInfo = new I_GameInfo();

    private async void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetGameInfo();
            await LoadRoomsAsync();
            await LoadMarksAsync();

            StartLifeUpdater();

            StartPositionLogger();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void StartPositionLogger()
    {
        while (true)
        {
            LogCurrentPosition();
            await Task.Delay(300000); 
        }
    }

    private void LogCurrentPosition()
    {
        if (OnlineMaps.instance != null)
        {
            double lat = OnlineMaps.instance.position.y;
            double lng = OnlineMaps.instance.position.x;

            XasuControl.Message("X: " + lat);
            XasuControl.Message("Y: "+ lng);

        }
        else
        {
            Debug.LogWarning("OnlineMaps.instance es null. ¿Está inicializado el mapa?");
        }
    }


    private async void StartLifeUpdater()
    {
        isRunningLifeUpdater = true;
        string userID = PlayerPrefs.GetString("UserID");

        while (isRunningLifeUpdater)
        {
            await RequestUserInfo.Instance.ModifyUserCritteronLifeTimeWithoutTimePass(userID);

            await Task.Delay(GetGameInfo().cureTime);
        }
    }

    private void OnDestroy()
    {
        isRunningLifeUpdater = false;
    }

    public static bool MarksReady()
    {
        return areMarksLoaded;
    }

    public static async Task LoadMarksAsync()
    {
        if (areMarksLoaded) return;

        cachedMarks = await RequestGameInfo.Instance.GetAllMarksAsync();
        areMarksLoaded = true;
    }

    public static async Task LoadRoomsAsync()
    {
        if (isDataLoaded) return;

        var roomsFromServer = await RequestGameInfo.Instance.GetAllRoomsAsync();
        roomPrices.Clear();
        cachedRooms.Clear();

        foreach (var room in roomsFromServer)
        {
            roomPrices.Add(room.price);
            cachedRooms.Add(room);
        }

        isDataLoaded = true;
    }

    public static void AddRoom(I_Room newRoom)
    {
        if (newRoom != null)
        {
            cachedRooms.Add(newRoom);
            roomPrices.Add(newRoom.price);
        }
    }

    public static List<I_Room> GetCachedRooms()
    {
        return cachedRooms;
    }

    public static List<float> GetRoomPrices()
    {
        return roomPrices;
    }

    public static List<I_Mark> GetCachedMarks()
    {
        return cachedMarks;
    }

    public static async void Reload()
    {
        isDataLoaded = false;
        roomPrices.Clear();
        cachedRooms.Clear();
        await LoadRoomsAsync();
    }


    void SetGameInfo()
    {
        gameInfo.cureTime = RequestGameInfo.Instance.GetCureTime();

        gameInfo.markTime = RequestGameInfo.Instance.GetMarkTime();
        gameInfo.reward = RequestGameInfo.Instance.GetReward();
        gameInfo.expPerCombat = RequestGameInfo.Instance.GetExpPerCombat();
        gameInfo.expGoal = RequestGameInfo.Instance.GetExpGoal();
        gameInfo.stepsToCombat = RequestGameInfo.Instance.GetStepsToCombat();
    }


    public static I_GameInfo GetGameInfo()
    {
        return gameInfo;
    }

    public static int GetTimemark()
    {
        if (time == 0)
            time = 100;
        return time;
    }


}
