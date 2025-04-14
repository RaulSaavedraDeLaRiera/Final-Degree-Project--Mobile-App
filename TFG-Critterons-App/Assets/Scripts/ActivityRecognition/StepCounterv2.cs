using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static I_UserInfo;

public class StepCounterV2 : MonoBehaviour
{
    public TextMeshProUGUI debugText; // UI para mostrar pasos
    private int initialSteps = 0;   // Valor inicial de pasos al iniciar
    private int initialStepsCalculate = 0;   // Valor inicial de pasos al iniciar
    private int currentSteps = 0;    // Pasos detectados en esta sesi?n
    private int steps = 0;    // Pasos detectados en esta sesi?n


    private InputAction stepAction;

    private bool permissionGranted = false;

    public int Steps => currentSteps;

    public static bool stepCounterWorking;

    bool delayInit = false;
    bool start = false;

    void Start()
    {
#if UNITY_ANDROID

        PlayerPrefs.SetInt("LastCombatSteps", 0);
        PlayerPrefs.Save();
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Pedir permisos de detecci?n de actividad (necesario en Android 10+)
        if (!Permission.HasUserAuthorizedPermission("android.permission.ACTIVITY_RECOGNITION"))
        {
            Permission.RequestUserPermission("android.permission.ACTIVITY_RECOGNITION");
            delayInit = true;
        }
        else
        {
            Debug.Log("Permiso concedido.");
            permissionGranted = true;
            InitializateSensor();
        }
#endif
    }


    void InitializateSensor()
    {
        InitializeStepCounter();
        // Verificar si el sensor est? disponible
        if (StepCounter.current != null)
        {
            Debug.Log("Sensor de pasos encontrado!");
            stepAction = new InputAction("StepCounter", InputActionType.Value, StepCounter.current.path);
            stepAction.performed += OnStepDetected;

            initialSteps = StepCounter.current.stepCounter.ReadValue();
            initialStepsCalculate = StepCounter.current.stepCounter.ReadValue();
            Debug.Log("Initial steps: " + initialSteps.ToString());

            stepCounterWorking = true;

            InvokeRepeating(nameof(CheckSteps), 30f, 30f);

        }
        else
        {
            stepCounterWorking = false;
            Debug.LogWarning("Sensor de pasos no encontrado en este dispositivo.");
        }
    }

    void Update()
    {
#if UNITY_ANDROID
        if (StepCounter.current != null)
        {

            int totalSteps = StepCounter.current.stepCounter.ReadValue();
            // Debug.Log("--> Sensores " + totalSteps.ToString());

            if (initialSteps == 0)
            {
                initialSteps = totalSteps;
            }

            currentSteps = totalSteps - initialSteps;
            steps = totalSteps - initialStepsCalculate;
            
            if (debugText != null)
                debugText.text = "Pasos: " + currentSteps;
        }
        else if (debugText != null)
        {
            debugText.text = "Sensor no disponible";
        }
#endif
    }

    private void OnStepDetected(InputAction.CallbackContext context)
    {
        int steps = (int)context.ReadValue<float>();

        //Debug.Log("Paso detectado!: " + steps);
    }

    void InitializeStepCounter()
    {
        InputSystem.EnableDevice(StepCounter.current);
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause && permissionGranted)
        {
            // Reinitialize the step counter when the app is resumed
            InitializeStepCounter();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (delayInit)
        {
            InitializateSensor();
            delayInit = false;
        }
    }

    void CheckSteps()
    {
        RequestUserInfo.Instance.GetUserByID(PlayerPrefs.GetString("UserID"), (user) =>
        {
            if (steps > 100)
                steps = 100;
            RequestUserInfoSocial.Instance.ModifyPersonalStats(PlayerPrefs.GetString("UserID"), globalSteps: user.personalStats.globalSteps + steps);
            steps = 0;
            initialStepsCalculate = StepCounter.current.stepCounter.ReadValue();
        });

    }
}