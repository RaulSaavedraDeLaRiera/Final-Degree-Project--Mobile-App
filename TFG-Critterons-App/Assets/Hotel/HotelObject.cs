using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelObject : MonoBehaviour
{
    // Deberia irse y hacer que un manager le diga lo que tiene que monstrar directamente ??
    // o que este a partir de la info que tiene cree el tipo de popup
    [SerializeField]
    GameObject popupPrefab;

    Vector3 popupPos;
    private bool isTouched;

    // Estado del objeto
    private bool isBougth;

    private void Start()
    {
        popupPos = new Vector3 (0, 0, 0);
        isTouched = false;

        // Cambiar para que pida la info de si lo esta a un manager?
        isBougth = false;
    }

    void Update()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                ShowPopup();
            }
        }
#elif UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.transform == transform)
                {
                    isTouched = true;
                }
            }
            else if (touch.phase == TouchPhase.Ended && isTouched)
            {
                ShowPopup();
                isTouched = false;
            }
        }
#endif
    }

    private void ShowPopup()
    {
        if (popupPrefab != null)
        {
           // GameObject popUp = Instantiate(popupPrefab, transform.position + popupPos, Quaternion.identity);
           // popUp.transform.SetParent(GameObject.Find("CanvasHotelUI").transform, false);
        }
    }
}

