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

    private async void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            await LoadRoomsAsync();
            await LoadMarksAsync();
        }
        else
        {
            Destroy(gameObject);
        }
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

        foreach (var mark in cachedMarks)
        {
            Debug.Log($"Marca cargada: {mark.name} {mark.x} {mark.y}");
        }
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
}
