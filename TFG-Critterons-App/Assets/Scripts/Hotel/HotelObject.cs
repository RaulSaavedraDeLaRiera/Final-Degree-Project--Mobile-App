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
    public bool Bought
    {
        get { return bought; }
    }
    public HotelObjectType TypeHotelObject
    {
        get { return typeHotelObject; }
    }

    private void Awake()
    {
        gameObject.tag = "HotelObject";
        originMat = visuals[0].material;
    }

    public void InitialiceObject(bool bought)
    {
        this.bought = bought;

        if (!bought)
        {
            foreach (var item in visuals)
                item.material = nonBoughtMat;
        }

    }

}

public enum HotelObjectType { decorationObject, cureObject }

