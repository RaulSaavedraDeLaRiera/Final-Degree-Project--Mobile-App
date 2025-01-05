using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SocialInfoSetter : MonoBehaviour
{
    [SerializeField]
    GameObject friend;

    void Start()
    {

    }

    void AddNewFriend()
    {
        SceneManager.LoadScene("AddFriend");
    }

}
