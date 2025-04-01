using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using static I_UserInfo;

public class StepCounterV2 : MonoBehaviour
{
    public TextMeshProUGUI debugText; // UI para mostrar pasos
    private int initialSteps = 0;   // Valor inicial de pasos al iniciar
    private int currentSteps = 0;    // Pasos detectados en esta sesi?n


    private InputAction stepAction;

    private bool permissionGranted = false;

    public int Steps => currentSteps;

    void Start()
    {
#if UNITY_ANDROID

        PlayerPrefs.SetInt("LastCombatSteps", 0);
        PlayerPrefs.Save();

        // Pedir permisos de detecci?n de actividad (necesario en Android 10+)
        if (!Permission.HasUserAuthorizedPermission("android.permission.ACTIVITY_RECOGNITION"))
        {
            Permission.RequestUserPermission("android.permission.ACTIVITY_RECOGNITION");
        }
        else
        {
            Debug.Log("Permiso concedido.");
            permissionGranted = true;
            InitializeStepCounter();
        }

        // Verificar si el sensor est? disponible
        if (StepCounter.current != null)
        {
            Debug.Log("Sensor de pasos encontrado!");
            stepAction = new InputAction("StepCounter", InputActionType.Value, StepCounter.current.path);
            stepAction.performed += OnStepDetected;

            initialSteps = StepCounter.current.stepCounter.ReadValue();
            Debug.Log("Initial steps: " + initialSteps.ToString());

            InvokeRepeating(nameof(CheckSteps), 40f, 40f);

        }
        else
        {
            Debug.LogWarning("Sensor de pasos no encontrado en este dispositivo.");
        }
#endif
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

    void CheckSteps()
    {
        RequestUserInfo.Instance.GetUserByID(PlayerPrefs.GetString("UserID"), (user) =>
        {
            RequestUserInfoSocial.Instance.ModifyPersonalStats(PlayerPrefs.GetString("UserID"), user.personalStats.globalSteps + currentSteps);
            currentSteps = 0;
        });

    }
}