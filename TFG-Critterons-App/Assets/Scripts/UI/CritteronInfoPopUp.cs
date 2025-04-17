using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CritteronInfoPopUp : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameC, healthC, descriptionC, attack1, attack2, attackDescription1, attackDescription2;
    [SerializeField]
    Slider healthS;

    public void AssignInfo(I_Critteron cInfo)
    {

        RequestUserInfo.Instance.GetUserCritteronsByID(PlayerPrefs.GetString("UserID"), cInfo.id, critteronUser =>
        {
            nameC.text = cInfo.name;
            healthC.text = critteronUser.currentLife.ToString();


            descriptionC.text = "lvl." + critteronUser.level.ToString() + "\n" +
                "damage :" + cInfo.basicDamage + "\n" +
                 "defense :" + (cInfo.defense * 100);

            if (cInfo.attacks[0] != null)
            {

                attack1.text = cInfo.attacks[0].name;
                attackDescription1.text =  "+" + cInfo.attacks[0].damage;
            }
            if (cInfo.attacks[1] != null)
            {
                attack2.text = cInfo.attacks[1].name;
                attackDescription2.text = "+" + cInfo.attacks[1].damage;
            }
             

            healthS.value = 3 / (float)cInfo.life;
        });
    }
}
