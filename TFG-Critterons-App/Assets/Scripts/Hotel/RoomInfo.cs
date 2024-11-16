using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [SerializeField]
    List<HotelObject> fornitures;

    public void InitialiceRoom(List<string> boughtObjectsID)
    {
        foreach (var item in fornitures)
        {
            item.InitialiceObject(boughtObjectsID.Contains(item.name));
        }
    }
}
