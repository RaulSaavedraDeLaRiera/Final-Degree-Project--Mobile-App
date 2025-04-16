using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoPrincipalManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI levelT, expT;


    void Start()
    {
        int level = 0, exp = 0, expToLevelUp = 0;

        //conseguirlo

        levelT.text = level.ToString();
        expT.text = exp.ToString() + "/" + expToLevelUp.ToString();

    }
}
