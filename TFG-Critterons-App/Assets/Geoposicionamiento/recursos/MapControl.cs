using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Timeline;

using System.Threading.Tasks;
using Unity.VisualScripting;

public class MapControl : MonoBehaviour
{
    [SerializeField]
    double[] coordenates;
    List<Tuple<string, Coordenate>> marksCoordenates = new List<Tuple<string, Coordenate>>();
    [SerializeField]
    OnlineMaps mapBase;
    [SerializeField]
    OnlineMapsMarker3DManager marksManager;
    [SerializeField]
    GameObject markPrefab;
    [SerializeField]
    float defaultScale = 10;

    [SerializeField]
    float maxDistanceToInteractWithPoint = 0.1f;

    [SerializeField]
    InteractMarkBehaviour markBehaviour;

    List<OnlineMapsMarker3D> marks = new List<OnlineMapsMarker3D>();
    List<Mark3D> logicMarks = new List<Mark3D>();


    long timeSinceLastInteract = -1, timeToInteract;



    private void Start()
    {
        InvokeRepeating("CreateMarks",0.2f, 0.25f);
        InitTimeCheck();

        markBehaviour.MapControl = this;

    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    void InitTimeCheck()
    {
        //si ya esta inicialziado volvemos
        if (timeSinceLastInteract >= 0)
            return;

        //servidor
        timeSinceLastInteract = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 10;
        timeToInteract = 0;
    }

    void SetLastTimeInteract()
    {
        timeSinceLastInteract = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        timeToInteract = InfoCache.GetGameInfo().markTime;

    }

    async Task LoadMarks()
    {
        var marksWork = await RequestGameInfo.Instance.GetAllMarksAsync();


        foreach (var m in marksWork)
        {
            Debug.Log("MARKS: " + m.name);
            marksCoordenates.Add(new Tuple<string, Coordenate>(m.name, new Coordenate(m.x, m.y)));
        }
    }

    void LoadCacheMarks()
    {
        var marksWork = InfoCache.GetCachedMarks();


        foreach (var m in marksWork)
        {
            Debug.Log("MARKS: " + m.name);
            marksCoordenates.Add(new Tuple<string, Coordenate>(m.name, new Coordenate(m.y, m.x)));
        }
    }

    void CreateMarks()
    {
        if (!InfoCache.MarksReady())
            return;

        CancelInvoke("CreateMarks");

        //carga de paradas
        //await LoadMarks();
        LoadCacheMarks();
        Debug.Log("marksReady");

        /*
        for (int i = 0; i < coordenates.Length; i += 2)
        {
            marksCoordenates.Add(new Tuple<string, Coordenate>("PARADA", new Coordenate(coordenates[i], coordenates[i + 1])));
        }
        */

        foreach (var mark in marksCoordenates)
        {
            var marker3D =
                marksManager.Create(mark.Item2.x, mark.Item2.y, markPrefab);
            marker3D.scale = defaultScale;
            var logicMark = marker3D.instance.GetComponent<Mark3D>();
            logicMark.SetParams(mark.Item1);

            logicMarks.Add(logicMark);
            marks.Add(marker3D);
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryRaycast();
        }
    }


    void TryRaycast()
    {



        // Genera un rayo desde la posición de la cámara hacia la dirección de la vista
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Almacén para la información del impacto del raycast
        RaycastHit hit;

        // Realiza el raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider != null)
            {
                // Intenta obtener el componente Mark3D del objeto impactado
                OnlineMapsMarker3D mark3D = GetMark(hit.collider.gameObject);

                if (mark3D != null)
                {
                    // Si el objeto tiene el componente Mark3D
                    if (markBehaviour.Ready() && DateTimeOffset.UtcNow.ToUnixTimeSeconds() > timeSinceLastInteract + timeToInteract)
                    {
                        Debug.Log($"Impacto en objeto con Mark3D: {hit.collider.gameObject.name}");
                        TryInteractWithPoint(mark3D);
                    }
                }
                else
                {
                    Debug.Log($"Impacto en objeto sin Mark3D: {hit.collider.gameObject.name}");
                }
            }
        }
    }

    OnlineMapsMarker3D GetMark(GameObject container)
    {
        foreach (var mark in marks)
        {
            if (mark.instance == container)
                return mark;
        }

        return null;
    }
    bool ValidMark(OnlineMapsMarker3D mark)
    {

        if (Coordenate.Distance(new Coordenate(mapBase.position.x, mapBase.position.y),
            new Coordenate(mark.position.x, mark.position.y))
            > maxDistanceToInteractWithPoint)
            return false;

        //tiempo entre uso de la marca
        return true;

    }

    void TryInteractWithPoint(OnlineMapsMarker3D mark)
    {
        if (!ValidMark(mark))
            return;

        Debug.Log("Marca interactuada!");
        XasuControl.MessageWithCustomVerb(
            actionId: "MARK_TOUCH",
            verbId: "http://adlnet.gov/expapi/verbs/interacted",
            verbDisplay: "interacted",
            timestamp: DateTime.UtcNow
        );

        SetLastTimeInteract();

        int[] rewards = { InfoCache.GetGameInfo().reward };
        mark.instance.GetComponent<Mark3D>().Interact(markBehaviour, rewards);
    }

    public async void InteractionComplete(int[] rewards)
    {
        SetLastTimeInteract();
        var user = await RequestUserInfo.Instance.GetUserAsync(PlayerPrefs.GetString("UserID"));
        RequestUserInfo.Instance.ModifyUserData(PlayerPrefs.GetString("UserID"), money: user.userData.money + rewards[0]);

        foreach (var item in logicMarks)
        {
            item.DisableMark(timeToInteract);
        }
    }


}

public class Coordenate
{
    public double x, y;

    public Coordenate(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public static double Distance(double x1, double y1, double x2, double y2)
    {
        double deltaX = x2 - x1;
        double deltaY = y2 - y1;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public static double Distance(Coordenate c1, Coordenate c2)
    {
        double deltaX = c2.x - c1.x;
        double deltaY = c2.y - c1.y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public double DistanceToThisPoint(double x, double y)
    {
        double deltaX = this.x - x;
        double deltaY = this.y - y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public double DistanceToThisPoint(Coordenate otherC)
    {
        double deltaX = x - otherC.x;
        double deltaY = y - otherC.y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
}
