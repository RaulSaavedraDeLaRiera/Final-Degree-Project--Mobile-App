using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelObject : MonoBehaviour
{
    [SerializeField]
    bool bought = false;
    [SerializeField]
    MeshRenderer[] visuals;
    [SerializeField]
    Material nonBoughtMat;
    [SerializeField]
    HotelObjectType typeHotelObject;
    [SerializeField]
    float actionValue;

    Material originMat;

    RoomInfo room;
    HotelCritteron currentUser;

    public bool Bought
    {
        get { return bought; }
    }
    public HotelObjectType TypeHotelObject
    {
        get { return typeHotelObject; }
    }
    public HotelCritteron CurrentUser
    {
        get { return currentUser; }
    }

    private void Awake()
    {
        gameObject.tag = "HotelObject";
        originMat = visuals[0].material;
    }

    public void InitialiceObject(bool bought, RoomInfo room, HotelManager hM)
    {
        this.bought = bought;
        this.room = room;

        if (!bought)
        {
            foreach (var item in visuals)
                item.material = nonBoughtMat;
        }
        else
            hM.AddObject(this, typeHotelObject);

    }



}

//Decoration siempre el ultimo
public enum HotelObjectType { cureObject, levelObject, decorationObject }

