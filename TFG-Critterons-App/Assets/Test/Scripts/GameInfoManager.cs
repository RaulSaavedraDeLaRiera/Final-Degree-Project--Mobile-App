using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameInfoManager : MonoBehaviour
{
    public static GameInfoManager Instance { get; private set; }

    private GameInfo gameInfo;
    private List<string> critteronIDs = new List<string>();
    private List<string> fornitureIDs = new List<string>();
    private List<string> userIDs = new List<string>();

    private void Awake()
    {
      
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        StartCoroutine(gameInfoRequest());
    }

    private IEnumerator gameInfoRequest()
    {
        string url = "http://localhost:8080/api/v1/gameinfo";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                gameInfo = JsonUtility.FromJson<GameInfo>(json);

                Debug.Log("Request successful. GameInfo ID: " + gameInfo.id);

                // Guardar los IDs de cada lista en las listas correspondientes
                critteronIDs.Clear();
                fornitureIDs.Clear();
                userIDs.Clear();

                foreach (var critteron in gameInfo.critterons)
                {
                    critteronIDs.Add(critteron.critteronID);
                }

                foreach (var forniture in gameInfo.forniture)
                {
                    fornitureIDs.Add(forniture.fornitureID);
                }

                foreach (var user in gameInfo.users)
                {
                    userIDs.Add(user.userID);
                }
            }
            else
            {
                Debug.LogError("Request failed: " + request.error);
            }
        }
    }

   
    public List<string> GetCritteronIDs() => critteronIDs;
    public List<string> GetFornitureIDs() => fornitureIDs;
    public List<string> GetUserIDs() => userIDs;
}
