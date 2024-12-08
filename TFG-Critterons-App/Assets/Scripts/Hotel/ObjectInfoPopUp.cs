using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInfoPopUp : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameT, descriptionT, bonusT;
    [SerializeField]
    Transform users;
    [SerializeField]
    HotelManager manager;

    RoomInfo targetRoom;
    ButtonActions buttonActions;

    private void Start()
    {
        buttonActions = GetComponent<ButtonActions>();
    }
    public void SetInfoPopUp(RoomInfo room)
    {
        nameT.text = room.NameRoom;
        descriptionT.text = room.Description;
        bonusT.text = room.TypeHotelRoom.ToString() + ": " + room.ValueRoom;
        if(users != null)
        {
            //hay que implementar que carguen foto critteron etc
            for (int i = 0; i < users.childCount; i++)
            {
                if(i < room.CritteronsInRoom.Count)
                users.GetChild(i).gameObject.SetActive(true);
                else
                users.GetChild(i).gameObject.SetActive(false);
            }
        }

        targetRoom = room;
    }

    public void TryBuy()
    {
        manager.TryBuyRoom(targetRoom, buttonActions);
    }


}
