using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelInput : MonoBehaviour
{
    //habra que cambiarlo a un popup segun tipo
    [SerializeField]
    ButtonActions critteronPopUp, ownedObjectPopUp, objectToBuyPopUp;
    [SerializeField]
    CritteronInfoPopUp infoCritteronPopUp;
    [SerializeField]
    ObjectInfoPopUp ownedPopUp, toBuyPopUp;
    [SerializeField]
    LayerMask interactuableLayer;

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
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactuableLayer))
        {
            GameObject target = hit.collider.gameObject;

            switch (target.tag)
            {
                case "Critteron":
                    var hCritteron = target.GetComponent<HotelCritteron>();

                    RequestUserInfo.Instance.ModifyUserCritteronLifeTime(PlayerPrefs.GetString("UserID"));
                    critteronPopUp.EnterAnimation();
                    AudioManager.m.PlaySound("click");
                    infoCritteronPopUp.AssignInfo(hCritteron.InfoCritteron);

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

                    var hObject = target.transform.parent.GetComponentInParent<RoomInfo>();

                    if(hObject.Bought)
                    {
                        ownedObjectPopUp.EnterAnimation();
                        AudioManager.m.PlaySound("click");
                        ownedPopUp.SetInfoPopUpBought(hObject);
                    }
                    else
                    { 
                        objectToBuyPopUp.EnterAnimation();
                        AudioManager.m.PlaySound("click");
                        toBuyPopUp.SetInfoPopUp(hObject);
                    }

                    inputEnable = false;
                    break;
            }
        }
    }
}
