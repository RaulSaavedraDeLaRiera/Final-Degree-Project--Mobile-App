using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;
    [SerializeField]
    float swayDuration = 3f;
    [SerializeField]
    float swayAmount = 15f;

    [SerializeField]
    GameObject critterons;

    [SerializeField]
    Image image;
        


    List<I_Critteron> possibleCritterons;

    void Start()
    {
        possibleCritterons = new List<I_Critteron>();

        getPossibleCritterons(); 

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 10f);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenCenter);

        GameObject instance = Instantiate(prefab, worldPosition, Quaternion.identity);

        instance.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
        StartCoroutine(SwayAndDestroy(instance));
    }

    IEnumerator SwayAndDestroy(GameObject instance)
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = instance.transform.position;

        while (elapsedTime < swayDuration)
        {
            float swayOffset = Mathf.Sin(elapsedTime * Mathf.PI * 2) * swayAmount;
            instance.transform.position = originalPosition + new Vector3(swayOffset, 0, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(instance);
        int randomIndex = Random.Range(0, possibleCritterons.Count);

        RequestUserInfo.Instance.ModifyUserCritteron(PlayerPrefs.GetString("UserID"), possibleCritterons[randomIndex].id);

        critterons.transform.Find(possibleCritterons[randomIndex].name).gameObject.SetActive(true);
        image.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Hotel");
    }

    void getPossibleCritterons()
    {
        RequestUserInfo.Instance.GetUserData(PlayerPrefs.GetString("UserID"), userdata =>
        {
            RequestGameInfo.Instance.GetAllCritteron(critterons =>
            {
                for (int i = 0; i < critterons.Count; i++)
                {
                    if (critterons[i].levelUnlock <= userdata.level)
                    {
                        possibleCritterons.Add(critterons[i]);
                    }
            }

            });
        });     
    }

}