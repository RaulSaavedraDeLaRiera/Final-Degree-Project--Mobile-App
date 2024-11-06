using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonActions : MonoBehaviour
{
    [SerializeField]
    ButtonAnimations enterAnimation = ButtonAnimations.none, outAnimation = ButtonAnimations.none;
    [SerializeField]
    PivotPosition pivotPosition = PivotPosition.up;
    [SerializeField]
    float timeEnterAnimation = 1f, timeOutAnimation = 1f;
    [SerializeField]
    GameObject[] objectsAffected;
    [SerializeField]
    bool startsEnable = false;


    Vector3 scale, position;
    private void Start()
    {
        scale = transform.localScale;
        position = transform.position;

        if (!startsEnable)
            gameObject.SetActive(false);
    }

    public void EnterAnimation()
    {
        //activamos el objeto;
        gameObject.SetActive(true);

        foreach (var obj in objectsAffected)
            obj.gameObject.SetActive(false);

        switch (enterAnimation)
        {
            case ButtonAnimations.none:
                break;

            case ButtonAnimations.popup:
                //lo colocamos en su posicion deseada y aseguramos que la escala este a 0
                transform.localScale = Vector3.zero;
                transform.position = position;
                //lo escalamos
                transform.DOScale(scale, timeEnterAnimation);
                break;

            case ButtonAnimations.slide:

                //aseguramos que su escala sea la original
                transform.localScale = scale;

                switch (pivotPosition)
                {
                    case PivotPosition.up:
                        transform.position = position + new Vector3(0, -Screen.height, 0);
                        break;
                    case PivotPosition.right:
                        transform.position = position + new Vector3(Screen.width, 0, 0);
                        break;
                    case PivotPosition.down:
                        transform.position = position + new Vector3(0, Screen.height, 0);
                        break;
                    case PivotPosition.left:
                        transform.position = position + new Vector3(-Screen.width, 0, 0);
                        break;
                }

                transform.DOMove(position, timeEnterAnimation);
                break;
        }
    }

    public void OutAnimation()
    {

        //Desactivamos el objeto una vez pasada la animacion

        switch (outAnimation)
        {
            case ButtonAnimations.none:
                DesactiveObject();
                break;
            case ButtonAnimations.popup:
                transform.DOScale(Vector3.zero, timeOutAnimation).onComplete = DesactiveObject;
                break;
            case ButtonAnimations.slide:
                switch (pivotPosition)
                {
                    case PivotPosition.up:
                        transform.DOMove(position + new Vector3(0, -Screen.height, 0), timeOutAnimation);
                        break;
                    case PivotPosition.right:
                        transform.DOMove(position + new Vector3(Screen.width, 0, 0), timeOutAnimation);
                        break;
                    case PivotPosition.down:
                        transform.DOMove(position + new Vector3(0, Screen.height, 0), timeOutAnimation);
                        break;
                    case PivotPosition.left:
                        transform.DOMove(position + new Vector3(-Screen.width, 0, 0), timeOutAnimation);
                        break;
                }
                Invoke("DesactiveObject", timeOutAnimation);

                break;
        }
    }

    void DesactiveObject()
    {
        gameObject.SetActive(false);
        foreach (var obj in objectsAffected)
            obj.gameObject.SetActive(true);
    }


    enum ButtonAnimations
    {
        none, popup, slide
    }
    enum PivotPosition
    {
        up, right, down, left
    }
}
