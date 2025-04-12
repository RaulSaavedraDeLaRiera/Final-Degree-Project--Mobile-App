using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static I_UserInfo;
using System;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float swayDuration = 3f;
    [SerializeField] float swayAmount = 15f;

    [SerializeField] GameObject critterons;
    [SerializeField] Image image;

    List<I_Critteron> possibleCritterons;
    GameObject instance;
    bool isSwaying = true;
    I_User user;

    async void Start()
    {
        possibleCritterons = new List<I_Critteron>();

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 10f);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenCenter);
        instance = Instantiate(prefab, worldPosition, Quaternion.identity);
        instance.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);

        AudioManager.m.PlaySound("newlevel");

        StartCoroutine(SwayLoop(instance));

        await SetCombat();
        await GetPossibleCritterons();

        isSwaying = false;
        Destroy(instance);

        StartCoroutine(HandleCritteronSelection());
    }

    async Task SetCombat()
    {
        user = await RequestUserInfo.Instance.GetUserAsync(PlayerPrefs.GetString("UserID"));
    }

    public async Task GetPossibleCritterons()
    {
        List<I_Critteron> critteronsList = await RequestGameInfo.Instance.GetAllCritteronAsync();
        possibleCritterons.Clear();

        for (int i = 0; i < critteronsList.Count; i++)
        {
            if (critteronsList[i].levelUnlock <= user.userData.level)
            {
                possibleCritterons.Add(critteronsList[i]);
            }
        }
    }

    IEnumerator SwayLoop(GameObject instance)
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = instance.transform.position;

        while (isSwaying)
        {
            float swayOffset = Mathf.Sin(elapsedTime * Mathf.PI * 2) * swayAmount;
            instance.transform.position = originalPosition + new Vector3(swayOffset, 0, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator HandleCritteronSelection()
    {
        if (possibleCritterons == null || possibleCritterons.Count == 0)
        {
            Debug.LogError("possibleCritterons list is empty or null.");
            yield break;
        }

        int randomIndex = UnityEngine.Random.Range(0, possibleCritterons.Count);
        Debug.Log($"Randomly selected index: {randomIndex}, Critteron: {possibleCritterons[randomIndex].name}");

        RequestUserInfo.Instance.GetUserCritterons(PlayerPrefs.GetString("UserID"), list =>
        {
            bool idExists = list.Any(critteron => critteron.critteronID == possibleCritterons[randomIndex].id);

            if (!idExists)
            {
                XasuControl.MessageWithCustomVerb(
                    actionId: "User_" + user.id + "Unlock_Critteron" + possibleCritterons[randomIndex].id,
                    verbId: "https://w3id.org/xapi/seriousgames/verbs/unlocked",
                    verbDisplay: "unlocked",
                    timestamp: DateTime.UtcNow
                );
                RequestUserInfo.Instance.ModifyUserCritteron(PlayerPrefs.GetString("UserID"), possibleCritterons[randomIndex].id, currentLife: possibleCritterons[randomIndex].life, level: 1);
                RequestUserInfoSocial.Instance.ModifyPersonalStats(PlayerPrefs.GetString("UserID"), critteronsOwned: user.personalStats.critteronsOwned + 1);
            }
            else
            {
                XasuControl.MessageWithCustomVerb(
                    actionId: "User_" + user.id + "LevelUp_Critteron" + possibleCritterons[randomIndex].id,
                    verbId: "http://adlnet.gov/expapi/verbs/completed",
                    verbDisplay: "completed",
                    timestamp: DateTime.UtcNow
                );
                RequestUserInfo.Instance.GetUserCritteronsByID(PlayerPrefs.GetString("UserID"), possibleCritterons[randomIndex].id, critteronUser =>
                {
                    RequestUserInfo.Instance.ModifyUserCritteron(PlayerPrefs.GetString("UserID"), possibleCritterons[randomIndex].id, currentLife: possibleCritterons[randomIndex].life, level: critteronUser.level + 1);
                });
            }
        });

        // Mostrar el critteron
        critterons.transform.Find(possibleCritterons[randomIndex].name).gameObject.SetActive(true);
        image.gameObject.SetActive(true);

        AudioManager.m.PlaySound("win");

        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Hotel");
    }
}
