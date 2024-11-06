using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class SelectPanel : MonoBehaviour
{
    [SerializeField]
    bool panelEnable = false;
    [SerializeField]
    UnityEvent actionSelected;
    [SerializeField]
    GameObject visualSelected;
    [SerializeField]
    Transform objectsRoot;

    [SerializeField]
    GraphicRaycaster raycaster;

    public bool PanelEnable
    {
        set { panelEnable = value; }
    }

    List<GameObject> objectsInPanel = new List<GameObject>();

    private void Start()
    {


        for (int i = 0; i < objectsRoot.childCount; i++)
        {
            objectsInPanel.Add(objectsRoot.GetChild(i).gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (panelEnable && Input.GetMouseButtonDown(0))
        {

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };


            List<RaycastResult> results = new List<RaycastResult>();

            // Realiza el raycast
            raycaster.Raycast(pointerData, results);

            // Comprueba si algún objeto fue impactado
            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    if (objectsInPanel.Contains(result.gameObject))
                    {
                        ObjectSelected(result.gameObject);
                    }
                }
            }
        }
    }


    void ObjectSelected(GameObject targetObject)
    {
        if (actionSelected != null)
            actionSelected.Invoke();
        if (visualSelected != null)
            visualSelected.transform.position = targetObject.transform.position;
 
    }
}
