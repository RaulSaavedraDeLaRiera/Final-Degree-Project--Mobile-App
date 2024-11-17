using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelManager : MonoBehaviour
{
    [SerializeField]
    Transform roomsRoot, critteronsRoot;
    [SerializeField]
    GameObject critteronPrefab;
   
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
        InitialiceCritterons();
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
                var r = rooms[Random.Range(0, rooms.Count)];
                if (r.AvailableSpace)
                    room = r;

            } while (room == null);

            var hotelCritteron = Instantiate<GameObject>(critteronPrefab,
                room.EntryPoint.position, room.EntryPoint.rotation, critteronsRoot)
                .GetComponent<HotelCritteron>();

            hotelCritteron.InitialiceCritteron(critteron);
        }
    }
}
