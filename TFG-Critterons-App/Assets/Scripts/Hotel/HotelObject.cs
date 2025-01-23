using System;
using UnityEngine;

public class HotelObject : MonoBehaviour
{
    [SerializeField]
    HotelObjectType typeHotelObject;
    [SerializeField]
    float actionValue, timeInObject, timeInObjectVariation, outCritteronTime;
    [SerializeField]
    Transform userPosition;
    [SerializeField]
    MeshRenderer[] visuals;
    [SerializeField]
    Material nonBoughtMat;
   
    bool bought = false;

    Material originMat;

    RoomInfo room;
    HotelCritteron currentUser;
    Vector3 prevPosCritteron;

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
        set { currentUser = value; }
    }

    public RoomInfo Room
    {
        get { return room; }
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

    void OutCritteron()
    {
        currentUser.transform.position = prevPosCritteron;
        currentUser.ActivateCritteron(outCritteronTime);
        currentUser = null;
    }

  //falla si quiere subir al mismo no detecta que vuela a entrar
    private void OnTriggerEnter(Collider other)
    {
        if(currentUser != null && other.gameObject == currentUser.gameObject)
        {
            prevPosCritteron = currentUser.transform.position;
            currentUser.transform.position = userPosition.position;
            currentUser.transform.rotation = userPosition.rotation;
            currentUser.StopCritteron();

            Invoke("OutCritteron", timeInObject + UnityEngine.Random.Range(-timeInObjectVariation, timeInObjectVariation));
        }
    }



}

//Decoration siempre el ultimo
public enum HotelObjectType { cureObject, levelObject, decorationObject, damageUpObject }

