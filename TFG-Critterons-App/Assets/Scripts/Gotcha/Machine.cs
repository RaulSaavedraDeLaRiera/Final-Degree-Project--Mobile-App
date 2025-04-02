using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    GameObject instance;
    bool critteronsReady = false;

    void Start()
    {
        possibleCritterons = new List<I_Critteron>();

        // **Crear la pelota antes de esperar los datos**
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 10f);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenCenter);

        instance = Instantiate(prefab, worldPosition, Quaternion.identity);
        instance.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);

        // Iniciar animación y carga de datos en paralelo
        StartCoroutine(SwayUntilReady(instance));
        StartCoroutine(getPossibleCritterons());
    }

    IEnumerator SwayUntilReady(GameObject instance)
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = instance.transform.position;

        // **Mover la pelota mientras los critterons se cargan**
        while (!critteronsReady)
        {
            float swayOffset = Mathf.Sin(elapsedTime * Mathf.PI * 2) * swayAmount;
            instance.transform.position = originalPosition + new Vector3(swayOffset, 0, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // **Cuando los critterons estén listos, destruir la pelota y seguir**
        yield return new WaitForSeconds(0.5f); // Pequeña espera antes de continuar
        Destroy(instance);
        StartCoroutine(HandleCritteronSelection());
    }

    IEnumerator HandleCritteronSelection()
    {
        if (possibleCritterons == null || possibleCritterons.Count == 0)
        {
            Debug.LogError("possibleCritterons list is empty or null.");
            yield break;
        }

        int randomIndex = Random.Range(0, possibleCritterons.Count);
        Debug.Log($"Randomly selected index: {randomIndex}, Critteron: {possibleCritterons[randomIndex].name}");

        RequestUserInfo.Instance.GetUserCritterons(PlayerPrefs.GetString("UserID"), list =>
        {
            bool idExists = list.Any(critteron => critteron.critteronID == possibleCritterons[randomIndex].id);

            if (!idExists)
            {
                RequestUserInfo.Instance.ModifyUserCritteron(PlayerPrefs.GetString("UserID"), possibleCritterons[randomIndex].id, currentLife: possibleCritterons[randomIndex].life, level: 1);
            }
            else
            {
                RequestUserInfo.Instance.GetUserCritteronsByID(PlayerPrefs.GetString("UserID"), possibleCritterons[randomIndex].id, critteronUser =>
                {
                    RequestUserInfo.Instance.ModifyUserCritteron(PlayerPrefs.GetString("UserID"), possibleCritterons[randomIndex].id, currentLife: possibleCritterons[randomIndex].life, level: critteronUser.level + 1);
                });
            }
        });

        critterons.transform.Find(possibleCritterons[randomIndex].name).gameObject.SetActive(true);
        image.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Hotel");
    }

    IEnumerator getPossibleCritterons()
    {
        Debug.Log("getPossibleCritterons called");

        bool isCompleted = false;

        RequestUserInfo.Instance.GetUserData(PlayerPrefs.GetString("UserID"), userdata =>
        {
            Debug.Log($"User data retrieved. User level: {userdata.level}");

            RequestGameInfo.Instance.GetAllCritteron(critterons =>
            {
                Debug.Log($"Total critterons retrieved: {critterons.Count}");

                possibleCritterons.Clear();

                for (int i = 0; i < critterons.Count; i++)
                {
                    if (critterons[i].levelUnlock <= userdata.level)
                    {
                        possibleCritterons.Add(critterons[i]);
                        Debug.Log($"Added critteron: {critterons[i].name}, Level Unlock: {critterons[i].levelUnlock}");
                    }
                }

                Debug.Log($"Total possibleCritterons: {possibleCritterons.Count}");
                critteronsReady = true; // Marca que la información está lista
                isCompleted = true;
            });
        });

        yield return new WaitUntil(() => isCompleted);
    }
}
