using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelInput : MonoBehaviour
{
    //habra que cambiarlo a un popup segun tipo
    [SerializeField]
    ButtonActions critteronPopUp, ownedObjectPopUp, objectToBuyPopUp;

    bool inputEnable = true;

    public bool InputEnable
    {
        set { inputEnable  = value; }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputEnable && Input.GetMouseButtonDown(0)) // Detecta clic izquierdo del mouse
        {
            TryInput();
        }
    }

    void TryInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Realiza el raycast y verifica si impacta algún objeto con Collider
        if (Physics.Raycast(ray, out hit))
        {
            GameObject target = hit.collider.gameObject;

            switch (target.tag)
            {
                case "Critteron":
                    critteronPopUp.EnterAnimation();
                    inputEnable = false;
                    break;
                //case "OwnedObject":
                //    ownedObjectPopUp.EnterAnimation();
                //    inputEnable = false;
                //    break;
                //case "ObjectToBuy":
                //    objectToBuyPopUp.EnterAnimation();
                //    inputEnable = false;
                //    break;
                case "HotelObject":

                    var hObject = target.GetComponent<HotelObject>();

                    if(hObject.Bought)
                        ownedObjectPopUp.EnterAnimation();
                    else
                        objectToBuyPopUp.EnterAnimation();

                    inputEnable = false;
                    break;
            }
        }
    }
}
