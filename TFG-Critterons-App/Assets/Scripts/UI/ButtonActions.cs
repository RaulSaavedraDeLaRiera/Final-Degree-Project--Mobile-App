using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    [SerializeField]
    float timeAnimation = 1f;
    public void CloseButtonAnimate()
    {
        transform.DOScale(Vector3.zero, timeAnimation).onComplete = Desactive;
    }

    void Desactive()
    {
        gameObject.SetActive(false);
    }
}
