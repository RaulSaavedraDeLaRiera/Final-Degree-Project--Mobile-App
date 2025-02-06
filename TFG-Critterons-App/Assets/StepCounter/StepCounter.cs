using UnityEngine;
using System;

public class StepCounterV1 : MonoBehaviour
{
    // Singleton setup
    private static StepCounterV1 _instance;
    public static StepCounterV1 Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<StepCounterV1>();
                if (_instance == null)
                {
                    GameObject container = new GameObject("StepCounter");
                    _instance = container.AddComponent<StepCounterV1>();
                }
            }
            return _instance;
        }
    }

    [Header("Configuration")]
    public StepCounterConfig config;

    [Header("Runtime Variables")]
    [SerializeField] private float distanceWalked = 0f;
    [SerializeField] private int stepCount = 0;

    private Vector3 acceleration;
    private Vector3 prevAcceleration;
    private Vector3 smoothedAcceleration;
    private float lastStepTime = 0f;

    private void Start()
    {
        if (config == null)
        {
            Debug.LogError("Oops! StepCounterConfig is missing!");
            return;
        }
        prevAcceleration = Input.acceleration;
        smoothedAcceleration = Input.acceleration;
        StepDataHandler.Instance.CheckForNewDay();
    }

    private void Update()
    {
        if (config == null) return;
        DetectSteps();
        CalculateDistance();
        StepDataHandler.Instance.SaveDailySteps(stepCount);
    }

    private void DetectSteps()
    {
        acceleration = Input.acceleration;

        // Apply low-pass filter to smooth the acceleration data
        smoothedAcceleration = Vector3.Lerp(smoothedAcceleration, acceleration, config.smoothingFactor);

        // Calculate the delta (change) in acceleration
        float delta = (smoothedAcceleration - prevAcceleration).magnitude;


        // Check if the delta exceeds the threshold and enforce time between steps
        if (Time.time - lastStepTime >= config.minStepInterval && delta > config.threshold && delta < config.limitAcceleration)
        {
            stepCount++;
            lastStepTime = Time.time; // Update the time of the last detected step
            Debug.Log($"Step detected! Acceleration: {delta}");

        
        }
        else
        {
            Debug.Log($"Step not valid! Acceleration: {Math.Round(delta, 2)}");
            if (delta >= config.limitAcceleration)
                lastStepTime = Time.time;
        }
    

            prevAcceleration = smoothedAcceleration;
    }

    private void CalculateDistance()
    {
        distanceWalked = stepCount * config.stepLength;
    }

    public void CalibrateStepLength(float newStepLength)
    {
        if (newStepLength > 0)
        {
            config.stepLength = newStepLength;
            Debug.Log($"Step length calibrated to: {config.stepLength} meters");
        }
        else
        {
            Debug.LogWarning("Whoops! That's not a valid step length.");
        }
    }

    // Getter methods and data management
    public float GetDistanceWalked() => distanceWalked;
    public int GetStepCount() => stepCount;

    public void ResetStepData()
    {
        stepCount = 0;
        distanceWalked = 0f;
    }

    public void LoadStepData(int loadedStepCount)
    {
        stepCount = loadedStepCount;
        CalculateDistance();
    }
}
