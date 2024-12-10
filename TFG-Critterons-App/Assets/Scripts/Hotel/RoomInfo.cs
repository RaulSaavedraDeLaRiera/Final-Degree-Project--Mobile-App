using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{

    //no necesario si es por servidor, vale con que quede registrado por servidor
    [SerializeField]
    string nameRoom, description;
    [SerializeField]
    HotelObjectType typeHotelRoom;
    [SerializeField]
    float valueRoom;

    [SerializeField]
    bool bought = false;


    [SerializeField]
    List<HotelObject> rooms;
    [SerializeField]
    List<HotelCritteron> critteronsInRoom;
    [SerializeField]
    Teleport entryPointToCritterons;

    [SerializeField]
    Vector2 size;

    [SerializeField]
    GameObject nonBoughtCube;



    int numCritteronsInRoom = 0;
    //public bool AvailableSpace
    //{
    //    get { return numCritteronsInRoom < critteronsInRoom.Length; }
    //}

    public Transform EntryPoint
    {
        get { return entryPointToCritterons.transform; }
    }
    public Vector2 Size
    {
        get { return size; }
    }

    public string Description => description;
    public string NameRoom => nameRoom;

    public HotelObjectType TypeHotelRoom => typeHotelRoom;

    public float ValueRoom => valueRoom;

    public List<HotelCritteron> CritteronsInRoom => critteronsInRoom;
    public bool Bought => bought;

    void Start()
    {
        entryPointToCritterons.Room = this;
    }
    //List<string> boughtRoomsID, List<string> boughtObjectsID, HotelManager hM
    async public void InitialiceRoom(List<string> boughtRoomsID, HotelManager hM)
    {
        // Obtengo la room con la info
        var room = await RequestGameInfo.Instance.GetRoomByIDAsync(gameObject.name);
        nameRoom = room.name;
        valueRoom = room.price;

        if (boughtRoomsID.Contains(gameObject.name))
        {
            bought = true;
            foreach (var item in rooms)
            {
                item.InitialiceObject(true, this, hM);
            }
            nonBoughtCube.SetActive(false);
        }

        else
            nonBoughtCube.SetActive(true);
    }

    public void InitialiceRoom(HotelManager hM)
    {

        bought = true;
        foreach (var item in rooms)
        {
            item.InitialiceObject(true, this, hM);
        }

        nonBoughtCube.SetActive(false);

    }

    public void AddCritteron(HotelCritteron critteron)
    {
        critteron.CurrentRoom = this;

        numCritteronsInRoom++;

        critteronsInRoom.Add(critteron);

        //for (int i = 0; i < critteronsInRoom.Length; i++)
        //{
        //    if (critteronsInRoom[i] == null)
        //        critteronsInRoom[i] = critteron;
        //}
    }

    public void RemoveCritteron(HotelCritteron critteron)
    {
        numCritteronsInRoom--;

        critteronsInRoom.Remove(critteron);

        //for (int i = 0; i < critteronsInRoom.Length; i++)
        //{
        //    if (critteronsInRoom[i] == critteron)
        //        critteronsInRoom[i] = null;
        //}
    }
}
