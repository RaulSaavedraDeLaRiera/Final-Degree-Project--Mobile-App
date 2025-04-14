using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Timeline;

using System.Threading.Tasks;
using Unity.VisualScripting;

using System;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    [SerializeField] double[] coordenates;
    List<Tuple<string, Coordenate>> marksCoordenates = new List<Tuple<string, Coordenate>>();

    [SerializeField] OnlineMaps mapBase;
    [SerializeField] OnlineMapsMarker3DManager marksManager;
    [SerializeField] GameObject markPrefab;
    [SerializeField] float defaultScale = 10;
    [SerializeField] float maxDistanceToInteractWithPoint = 0.1f;
    [SerializeField] InteractMarkBehaviour markBehaviour;

    List<OnlineMapsMarker3D> marks = new List<OnlineMapsMarker3D>();
    List<Mark3D> logicMarks = new List<Mark3D>();

    long timeSinceLastInteract = -1, timeToInteract;

    private const string LastAvailableMarksTimestampKey = "LastAvailableMarksTimestamp";

    private void Start()
    {
        InvokeRepeating("CreateMarks", 0.2f, 0.25f);
        InitTimeCheck();
        markBehaviour.MapControl = this;
    }

    private void OnDestroy()
    {
        CancelInvoke();

        // Guarda el tiempo restante hasta que se puedan volver a usar las marcas
        if (timeSinceLastInteract > 0)
        {
            long nextAvailable = timeSinceLastInteract + timeToInteract;
            PlayerPrefs.SetString(LastAvailableMarksTimestampKey, nextAvailable.ToString());
            PlayerPrefs.Save();
        }
    }

    void InitTimeCheck()
    {
        if (timeSinceLastInteract >= 0) return;

        if (PlayerPrefs.HasKey(LastAvailableMarksTimestampKey))
        {
            long savedNextAvailable = long.Parse(PlayerPrefs.GetString(LastAvailableMarksTimestampKey));
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long remaining = savedNextAvailable - now;

            if (remaining > 0)
            {
                timeSinceLastInteract = now;
                timeToInteract = remaining;
                return;
            }
        }

        // Si no existe o ya expiró
        timeSinceLastInteract = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 10;
        timeToInteract = 0;
    }

    void SetLastTimeInteract()
    {
        timeSinceLastInteract = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        timeToInteract = InfoCache.GetGameInfo().markTime;
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
        if (!InfoCache.MarksReady()) return;

        CancelInvoke("CreateMarks");

        LoadCacheMarks();
        Debug.Log("marksReady");

        bool isActive = DateTimeOffset.UtcNow.ToUnixTimeSeconds() > timeSinceLastInteract + timeToInteract;

        foreach (var mark in marksCoordenates)
        {
            var marker3D = marksManager.Create(mark.Item2.x, mark.Item2.y, markPrefab);
            marker3D.scale = defaultScale;

            var logicMark = marker3D.instance.GetComponent<Mark3D>();
            logicMark.SetParams(mark.Item1);

           
            if (!isActive)
            {
                float remaining = (float)(timeSinceLastInteract + timeToInteract - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                logicMark.DisableMark(remaining);
            }
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider != null)
            {
                OnlineMapsMarker3D mark3D = GetMark(hit.collider.gameObject);
                if (mark3D != null)
                {
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
            if (mark.instance == container) return mark;
        }
        return null;
    }

    bool ValidMark(OnlineMapsMarker3D mark)
    {
        return Coordenate.Distance(new Coordenate(mapBase.position.x, mapBase.position.y),
            new Coordenate(mark.position.x, mark.position.y)) <= maxDistanceToInteractWithPoint;
    }

    void TryInteractWithPoint(OnlineMapsMarker3D mark)
    {
        if (!ValidMark(mark)) return;

        Debug.Log("Marca interactuada!");
        XasuControl.MessageWithCustomVerb(
            actionId: "Mark_used",
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
