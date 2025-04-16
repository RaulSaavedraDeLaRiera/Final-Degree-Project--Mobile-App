using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingAnimation : MonoBehaviour
{
    [SerializeField]
    GameObject waitingCanvas;

    public void Hide(float extraTime = 0)
    {
        if (extraTime == 0)
            Hide();
        else
            Invoke("Hide", extraTime);

    }

    public void Show()
    {
        waitingCanvas.SetActive(true);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }


    void Hide()
    {
        AudioManager.m.PlaySound("changescene");
        waitingCanvas.SetActive(false);
    }
}
