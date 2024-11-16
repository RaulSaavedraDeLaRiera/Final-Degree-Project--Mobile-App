using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelCritteron : MonoBehaviour
{

    [SerializeField]
    CritteronInfo infoCritteron;

    [SerializeField]
    Transform visualRoot;

    public CritteronInfo InfoCritteron
    {
        get
        {
            return infoCritteron;
        }
    }

    //animato etc

    public void InitialiceCritteron(CritteronInfo info)
    {
        infoCritteron = info;

        gameObject.name = infoCritteron.name;

        var visual = visualRoot.Find(infoCritteron.mesh);

        visual.gameObject.SetActive(true);
    }
}
