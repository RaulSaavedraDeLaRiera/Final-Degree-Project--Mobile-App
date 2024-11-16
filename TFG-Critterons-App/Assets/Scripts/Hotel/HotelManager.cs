using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelManager : MonoBehaviour
{
    [SerializeField]
    Transform roomsRoot;
   
    List<RoomInfo> rooms = new List<RoomInfo>();
    List<Critteron> userCritterons;

    void Awake()
    {
        for (int i = 0; i < roomsRoot.childCount; i++)
        {
            rooms.Add(roomsRoot.GetChild(i).GetComponent<RoomInfo>());
        }
    }

    void Start()
    {
        InitialiceRooms();
    }

    void InitialiceRooms()
    {

        //datos desde servidor
        List<string> data = new List<string>();
        data.Add("1");
        data.Add("2");

        foreach (var room in rooms)
        {
            room.InitialiceRoom(data);
        }
    }
}
