using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    float percentRoom;

    
    const int roomSize = 2;

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

    int placesUsed = 0;
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
    public float PercentRoom => percentRoom;

    public List<HotelCritteron> CritteronsInRoom => critteronsInRoom;

    public bool Bought => bought;

    private CancellationTokenSource _cts;

    void OnEnable()
    {
        _cts = new CancellationTokenSource();  // Crear un nuevo token cuando el objeto se activa
    }

    void OnDisable()
    {
        _cts.Cancel(); // Cancelar cualquier operación en curso si el objeto se desactiva o destruye
    }

    void Start()
    {
        entryPointToCritterons.Room = this;
    }

    public void PlaceUsed()
    {
        placesUsed++;
        Debug.Log("current places used: " + placesUsed);
    }
    public bool AvailableSpace()
    {
        if (roomSize > placesUsed)
            return true;
        else
            return false;
        
    }
    //List<string> boughtRoomsID, List<string> boughtObjectsID, HotelManager hM
    async public void InitialiceRoom(List<string> boughtRoomsID, HotelManager hM)
    {
        if (_cts.Token.IsCancellationRequested) return;  // Si se canceló antes de empezar, salir

        try
        {
            // Obtener la información de la habitación
            var room = await RequestGameInfo.Instance.GetRoomByIDAsync(gameObject.name, _cts);

            if (_cts.Token.IsCancellationRequested || this == null || gameObject == null) return; // Verificar si fue destruido

            nameRoom = room.name;
            valueRoom = room.price;
            percentRoom = room.percent;

            if (boughtRoomsID.Contains(gameObject.name))
            {
                bought = true;
                foreach (var item in rooms)
                {
                    item.InitialiceObject(this, hM);
                }
                nonBoughtCube.SetActive(false);
            }
            else
            {
                nonBoughtCube.SetActive(true);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("?? Request cancelled because object was destroyed.");
        }
    }


    public void InitialiceRoom(HotelManager hM)
    {

        bought = true;
        foreach (var item in rooms)
        {
            item.InitialiceObject(this, hM);
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
