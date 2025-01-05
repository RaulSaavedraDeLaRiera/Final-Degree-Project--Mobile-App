using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    static float TimeToAllowTeleports = 0.5f;
    RoomInfo room;
    Collider collider;

    public RoomInfo Room
    {
        set { room = value; }
    }

    private void Start()
    {
        collider = GetComponent<Collider>();

        Invoke("EnableTeleport", TimeToAllowTeleports);
    }
    public void EnableTeleport()
    {
        collider.enabled = true;
    }

    public void DisableTeleport()
    {
        collider.enabled = false;
    }


    private void OnTriggerEnter(Collider other)
    {


        var critteron = other.GetComponent<HotelCritteron>();
        Debug.Log("TELEPORT");
        if(critteron != null && 
            critteron.Target != null && critteron.CurrentRoom != critteron.Target.Room)
        {
            critteron.ChangePosition(critteron.Target.Room.EntryPoint.position);
            critteron.Target.Room.AddCritteron(critteron);
            critteron.CurrentRoom.RemoveCritteron(critteron);

            critteron.ForceUpdate(0.25f);
        }
    }
}
