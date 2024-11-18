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
    Transform entryPointToCritterons;

    [SerializeField]
    float xSize, zSize;

    int numCritteronsInRoom = 0;
    public bool AvailableSpace
    {
        get { return numCritteronsInRoom < critteronsInRoom.Length; }
    }

    public Transform EntryPoint
    {
        get { return entryPointToCritterons; }
    }
    public Tuple<float, float> Size
    {
        get { return new Tuple<float, float>(xSize, zSize); }
    }

    public void InitialiceRoom(List<string> boughtObjectsID, HotelManager hM)
    {
        foreach (var item in fornitures)
        {
            item.InitialiceObject(boughtObjectsID.Contains(item.name), this, hM);
        }

       
    }

    void AddCritteron(HotelCritteron critteron)
    {
        numCritteronsInRoom++;

        for (int i = 0; i < critteronsInRoom.Length; i++)
        {
            if (critteronsInRoom[i] == null)
                critteronsInRoom[i] = critteron;
        }
    }

    void RemoveCritteron(HotelCritteron critteron)
    {
        numCritteronsInRoom--;

        for (int i = 0; i < critteronsInRoom.Length; i++)
        {
            if (critteronsInRoom[i] == critteron)
                critteronsInRoom[i] = null;
        }
    }
}
