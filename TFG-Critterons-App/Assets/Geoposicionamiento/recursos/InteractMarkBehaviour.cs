using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class InteractMarkBehaviour : MonoBehaviour
{
    [SerializeField]
    GameObject markUI;
    [SerializeField]
    TextMeshProUGUI textName, textRewards;
    [SerializeField]
    float enterTime = 0.5f, animationTime = 1, animationSpeed = 10;


    MapControl control;
    public MapControl MapControl
    {
        set
        {
            control = value;
        }
    }

    bool interactInProgress, interactStarted;
    int[] rewards;


    private void Update()
    {
        if (interactInProgress)
            markUI.transform.rotation *= Quaternion.Euler(0, 0, animationSpeed * Time.timeSinceLevelLoad);
    }

    public bool Ready()
    {
        return !interactStarted && !interactInProgress;
    }

    public void SetMark(string textName, int[] rewards)
    {
        markUI.transform.localScale = Vector3.zero;
        markUI.SetActive(true);
        markUI.transform.DOScale(1, enterTime);
        interactStarted = true;


        markUI.transform.rotation = Quaternion.Euler(0, 0, 0);


        this.textName.text = textName;
        this.rewards = rewards;

        //de momento siempre es monedas
        textRewards.text = rewards[0].ToString();


        AudioManager.m.PlaySound("stop");
    }

    public void Interact()
    {
        //si ya se esta haciendo o aun no se puede
        if (interactInProgress || markUI.transform.localScale.x < 1)
            return;

        interactInProgress = true;
        Debug.Log("interact with: " + textName.text);


        markUI.transform.DOScale(0, animationTime).onComplete = EndInteract;

        AudioManager.m.PlaySound("stop");

    }

    void EndInteract()
    {
        markUI.SetActive(false);
        interactInProgress = interactStarted = false;
        control.InteractionComplete(rewards);
    }
}
