//using UnityEngine.Networking;
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class EntityCreator : MonoBehaviour
//{
//    [SerializeField]
//    private GameInfoManager gameInfoManager;

//    private List<FornitureInfo> fornitureInfos = new List<FornitureInfo>();
//    private List<CritteronInfo> critteronInfos = new List<CritteronInfo>();
//    private List<UserInfo> userInfos = new List<UserInfo>();

//    private void Start()
//    {
//    }
    

//    public void Init()
//    {
//        StartCoroutine(RequestCritteronData());
//        StartCoroutine(RequestFornitureData());
//        StartCoroutine(RequestUserData());
//    }

//    private IEnumerator RequestCritteronData()
//    {
//        foreach (string critteronID in gameInfoManager.GetCritteronIDs())
//        {
//            string url = $"http://localhost:8080/api/v1/critteron/{critteronID}";
//            using (UnityWebRequest request = UnityWebRequest.Get(url))
//            {
//                yield return request.SendWebRequest();

//                if (request.result == UnityWebRequest.Result.Success)
//                {
//                    string json = request.downloadHandler.text;
//                    Debug.Log($"Critteron Data for ID {critteronID}: {json}");
//                    CritteronInfo critteron = JsonUtility.FromJson<CritteronInfo>(json);
//                    critteronInfos.Add(critteron);
//                }
//                else
//                {
//                    Debug.LogError($"Failed to fetch Critteron data for ID {critteronID}: {request.error}");
//                }
//            }
//        }
//    }

//    private IEnumerator RequestFornitureData()
//    {
//        foreach (string fornitureID in gameInfoManager.GetFornitureIDs())
//        {
//            string url = $"http://localhost:8080/api/v1/forniture/{fornitureID}";
//            using (UnityWebRequest request = UnityWebRequest.Get(url))
//            {
//                yield return request.SendWebRequest();

//                if (request.result == UnityWebRequest.Result.Success)
//                {
//                    string json = request.downloadHandler.text;
//                    Debug.Log($"Forniture Data for ID {fornitureID}: {json}");
//                    FornitureInfo forniture = JsonUtility.FromJson<FornitureInfo>(json);
//                    fornitureInfos.Add(forniture);
//                }
//                else
//                {
//                    Debug.LogError($"Failed to fetch Forniture data for ID {fornitureID}: {request.error}");
//                }
//            }
//        }
//    }

//    private IEnumerator RequestUserData()
//    {
//        foreach (string userID in gameInfoManager.GetUserIDs())
//        {
//            string url = $"http://localhost:8080/api/v1/user/{userID}";
//            using (UnityWebRequest request = UnityWebRequest.Get(url))
//            {
//                yield return request.SendWebRequest();

//                if (request.result == UnityWebRequest.Result.Success)
//                {
//                    string json = request.downloadHandler.text;
//                    Debug.Log($"User Data for ID {userID}: {json}");
//                    UserInfo user = JsonUtility.FromJson<UserInfo>(json);
//                    userInfos.Add(user);
//                }
//                else
//                {
//                    Debug.LogError($"Failed to fetch User data for ID {userID}: {request.error}");
//                }
//            }
//        }
//    }


//}
