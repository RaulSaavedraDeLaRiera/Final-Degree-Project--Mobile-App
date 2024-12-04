using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowSteps : MonoBehaviour
{
    [SerializeField]
    float updateFrecuence = 0.5f;
    [SerializeField]
    TextMeshProUGUI text;

    private void Start()
    {
        InvokeRepeating("ActualizateText", 0, updateFrecuence);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
    void ActualizateText()
    {
        text.text = StepCounter.Instance.GetStepCount().ToString();
    }
}
