using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StepCounterConfig", menuName = "StepCounter/Config", order = 1)]
public class StepCounterConfig : ScriptableObject
{
    [Header("Step Counter Settings")]
    [Tooltip("Average step length in meters.")]
    public float stepLength = 0.75f;

    [Header("Detection Settings")]
    [Tooltip("Acceleration threshold for detecting steps.")]
    public float threshold = 1f;

    [Tooltip("Maxima aceleración para evitar falsos pasos.")]
    public float limitAcceleration = 2f;

    [Tooltip("Minimum time interval between steps in seconds.")]
    public float minStepInterval = 0.5f;

    [Tooltip("Factor de suavizado para evita ruido en los pasos. De 0 a 1.")]
    public float smoothingFactor = 0.1f;
}
