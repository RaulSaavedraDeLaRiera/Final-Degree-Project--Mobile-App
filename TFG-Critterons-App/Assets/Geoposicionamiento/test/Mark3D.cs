using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark3D : MonoBehaviour
{
    [SerializeField]
    string markName;

    [SerializeField]
    Transform animationPart;

    public void SetParams(string name)
    {
        markName = name;
    }
    public void Interact(InteractMarkBehaviour behaviour, int[] rewards)
    {
        behaviour.SetMark(markName, rewards);
    }
}
