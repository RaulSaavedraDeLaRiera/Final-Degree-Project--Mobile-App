using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField]
    string[] scenesAllowed;


    public void LoadSceneSync(int i)
    {
        SceneManager.LoadScene(scenesAllowed[i]);
    }
}
