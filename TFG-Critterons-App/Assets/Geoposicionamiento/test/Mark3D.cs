using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark3D : MonoBehaviour
{
    [SerializeField]
    string markName;

    [SerializeField]
    Transform animationPart;

    [SerializeField]
    MeshRenderer[] modColor;
    [SerializeField]
    Material enableColor, disableColor;

    public void SetParams(string name)
    {
        markName = name;
    }
    public void Interact(InteractMarkBehaviour behaviour, int[] rewards)
    {
        behaviour.SetMark(markName, rewards);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    public void DisableMark(float timeToEnable)
    {
        foreach (var item in modColor)
        {
            item.material = disableColor;
            Invoke("EnableMark", timeToEnable);
        }

    }

    void EnableMark()
    {
        foreach (var item in modColor)
        {
            item.material = enableColor;
        }
    }
}
