using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark3D : MonoBehaviour
{
    [SerializeField]
    string markName;

    [SerializeField]
    Transform bodyPart, animationPart;

    [SerializeField]
    MeshRenderer[] modColor;
    [SerializeField]
    Material enableColor, disableColor;

    [SerializeField]
    Material[] baseMats;


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

    private void Start()
    {
        baseMats = new Material[modColor.Length];
        for (int i = 0; i < modColor.Length; i++)
        {
            baseMats[i] = modColor[i].material;
        }
        if (bodyPart != null)
            bodyPart.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }


    private void Update()
    {
        if (animationPart != null)
        {
            Vector3 direction = Camera.main.transform.position - transform.position;
            direction.y = 0; // Mantiene el objeto recto
            animationPart.rotation = Quaternion.LookRotation(direction);
        }
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

        for (int i = 0; i < baseMats.Length; i++)
        {
            modColor[i].material = baseMats[i];
        }
    }
}
