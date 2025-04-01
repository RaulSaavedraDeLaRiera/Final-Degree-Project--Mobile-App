using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RequestGameInfo : MonoBehaviour
{
    private static RequestGameInfo _instance;

    public static RequestGameInfo Instance
    {
        get
        {
            if (_instance == null)
            {
                UnityEngine.Debug.LogError("RequestGameInfo instance is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // <summary>
    /// Obtener un critteron por ID
    /// </summary>
    public void GetCritteronByID(string id, Action<I_Critteron> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetCritteronByID(id, callback));
    }

    public Task<I_Critteron> GetCritteronByIDAsync(string critteronID)
    {
        var tcs = new TaskCompletionSource<I_Critteron>();

        RequestGameInfo.Instance.GetCritteronByID(critteronID, critteron =>
        {
            tcs.SetResult(critteron);
        });

        return tcs.Task;
    }

    /// <summary>
    /// Obtener todos los critterons
    /// </summary>
    public void GetAllCritteron(Action<List<I_Critteron>> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetAllCritteron(callback));
    }

    public Task<List<I_Critteron>> GetAllCritteronAsync()
    {
        var tcs = new TaskCompletionSource<List<I_Critteron>>();
        StartCoroutine(ServerConnection.Instance.GetAllCritteron(critterons =>
        {
            tcs.SetResult(critterons);
        }));
        return tcs.Task;
    }

    /// <summary>
    /// Obtener un room por ID
    /// </summary>
    public void GetRoomByID(string id, Action<I_Room> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetRoomByID(id, callback));
    }



    public Task<I_Room> GetRoomByIDAsync(string roomID, CancellationTokenSource cts = null)
    {
        var tcs = new TaskCompletionSource<I_Room>();

        // Si hay un CancellationToken, registramos la cancelación
        cts?.Token.Register(() =>
        {
            tcs.TrySetCanceled(); // Cancelamos la tarea si el token lo indica
        });

        RequestGameInfo.Instance.GetRoomByID(roomID, room =>
        {
            // Verificamos si la tarea ya fue cancelada antes de completar el resultado
            if (cts?.Token.IsCancellationRequested == true)
            {
                tcs.TrySetCanceled();
                return;
            }

            tcs.TrySetResult(room);
        });

        return tcs.Task;
    }


    /// <summary>
    /// Obtener todos los room
    /// </summary>
    public void GetAllRooms(Action<List<I_Room>> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetAllRooms(callback));
    }

    public void GetMarkByID(string id, Action<I_Mark> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetMarkByID(id, callback));
    }

    public Task<I_Mark> GetMarkByIDAsync(string markID)
    {
        var tcs = new TaskCompletionSource<I_Mark>();

        RequestGameInfo.Instance.GetMarkByID(markID, mark =>
        {
            tcs.SetResult(mark);
        });

        return tcs.Task;
    }

    public void GetAllMarks(Action<List<I_Mark>> callback)
    {
        StartCoroutine(ServerConnection.Instance.GetAllMarks(callback));
    }

    public Task<List<I_Room>> GetAllRoomsAsync()
    {
        var tcs = new TaskCompletionSource<List<I_Room>>();

        GetAllRooms(roomsResult =>
        {
            tcs.SetResult(roomsResult);
        });

        return tcs.Task;
    }

    public Task<List<I_Mark>> GetAllMarksAsync()
    {
        var tcs = new TaskCompletionSource<List<I_Mark>>();

        GetAllMarks(marksResult =>
        {
            tcs.SetResult(marksResult);
        });

        return tcs.Task;
    }

    public int GetCureTime()
    {
        return ServerConnection.Instance.GetGameInfo().cureTime;
    }
}
