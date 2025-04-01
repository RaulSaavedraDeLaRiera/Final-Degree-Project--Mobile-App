using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField]
    string[] scenesAllowed;
    [SerializeField]
    float limitTime = 0;


    public void LoadSceneSync(int i)
    {
        if (Time.timeSinceLevelLoad < limitTime)
            return;

        SceneManager.LoadScene(scenesAllowed[i]);
    }
}
