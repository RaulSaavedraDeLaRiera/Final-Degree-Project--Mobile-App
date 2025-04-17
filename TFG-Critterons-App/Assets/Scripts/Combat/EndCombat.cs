using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCombat : MonoBehaviour
{
    [SerializeField]
    GameObject win;

    [SerializeField]
    GameObject lose;

    [SerializeField]
    TextMeshProUGUI infoText;

    [SerializeField]
    Camera mainCamera;

    void Start()
    {
        int result = PlayerPrefs.GetInt("Result");

        if (result == 0 )
        {
            if(PlayerPrefs.GetString("EXP") == "true")
                infoText.text = "MONEY +" + InfoCache.GetGameInfo().reward.ToString() + "\n" + "EXP +" + InfoCache.GetGameInfo().expPerCombat.ToString();
            win.SetActive(true);
            AudioManager.m.PlaySound("win");
        }
        else
        {
            lose.SetActive(true);
            mainCamera.backgroundColor = Color.red;
            AudioManager.m.PlaySound("defeat");
        }
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Hotel");
    }

}
