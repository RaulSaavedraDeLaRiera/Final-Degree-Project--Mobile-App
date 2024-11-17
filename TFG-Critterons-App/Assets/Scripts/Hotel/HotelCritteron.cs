using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelCritteron : MonoBehaviour
{

    [SerializeField]
    I_Critteron infoCritteron;

    [SerializeField]
    Transform visualRoot;

    public I_Critteron InfoCritteron
    {
        get
        {
            return infoCritteron;
        }
    }

    //animato etc

    public void InitialiceCritteron(I_Critteron info)
    {
        infoCritteron = info;

        gameObject.name = infoCritteron.name;

        var visual = visualRoot.Find(infoCritteron.mesh);

        visual.gameObject.SetActive(true);
    }
}
