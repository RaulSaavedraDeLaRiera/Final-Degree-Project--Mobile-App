using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IdShareControl : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI IdText;
    // Start is called before the first frame update
    void Start()
    {
      IdText.text = PlayerPrefs.GetString("UserID");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
