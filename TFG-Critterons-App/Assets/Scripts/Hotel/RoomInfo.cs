using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [SerializeField]
    List<HotelObject> fornitures;
    [SerializeField]
    HotelCritteron[] critteronsInRoom;
    [SerializeField]
    Teleport entryPointToCritterons;

    [SerializeField]
    Vector2 size;

    [SerializeField]
    GameObject nonBoughtCube;

    //no necesario si es por servidor, vale con que quede registrado por servidor
    /*[SerializeField]
    HotelObjectType typeHotelRoom;
    [SerializeField]
    float valueRoom;*/

    int numCritteronsInRoom = 0;
    public bool AvailableSpace
    {
        get { return numCritteronsInRoom < critteronsInRoom.Length; }
    }

    public Transform EntryPoint
    {
        get { return entryPointToCritterons.transform; }
    }
    public Vector2 Size
    {
        get { return size; }
    }

    void Start()
    {
        entryPointToCritterons.Room = this;
    }

    public void InitialiceRoom(List<string> boughtRoomsID, List<string> boughtObjectsID, HotelManager hM)
    {

        if (boughtRoomsID.Contains(gameObject.name))
            foreach (var item in fornitures)
            {
                item.InitialiceObject(boughtObjectsID.Contains(item.name), this, hM);
            }
        else
            nonBoughtCube.SetActive(true);


    }

    public void AddCritteron(HotelCritteron critteron)
    {
        critteron.CurrentRoom = this;

        numCritteronsInRoom++;

        for (int i = 0; i < critteronsInRoom.Length; i++)
        {
            if (critteronsInRoom[i] == null)
                critteronsInRoom[i] = critteron;
        }
    }

    public void RemoveCritteron(HotelCritteron critteron)
    {
        numCritteronsInRoom--;

        for (int i = 0; i < critteronsInRoom.Length; i++)
        {
            if (critteronsInRoom[i] == critteron)
                critteronsInRoom[i] = null;
        }
    }
}
