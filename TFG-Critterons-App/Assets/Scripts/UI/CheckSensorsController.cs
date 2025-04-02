using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CheckSensorsController : MonoBehaviour
{

    [SerializeField]
    float timeBtwChecks;
    [SerializeField]
    GameObject screen, location, steps;
    [SerializeField]
    bool enableCheck;

    // Start is called before the first frame update
    void Start()
    {

        if (enableCheck)
            InvokeRepeating("CheckSensors", timeBtwChecks, timeBtwChecks);
    }


    void CheckSensors()
    {
        bool steps = StepCounterV2.stepCounterWorking, location = Input.location.isEnabledByUser;

        if (!steps || !location)
        {
            screen.SetActive(true);

            this.steps.active = this.location.active = false;

            if (!steps)
            {
                this.steps.active = true;
            }
            else if (!location)
            {
                this.location.active = true;
            }


        }
        else
            screen.SetActive(false);
    }
}
