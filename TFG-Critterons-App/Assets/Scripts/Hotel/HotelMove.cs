using System;
using UnityEngine;

/// <summary>
/// Clase encargada del desplazamiento por la pantalla del hotel
/// </summary>
public class HotelMove : MonoBehaviour
{
    [SerializeField]
    float hotelWidth;
    [SerializeField]
    float hotelHeight;

    [SerializeField]
    float moveSpeed;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private Vector3 lastPos;
    private bool isDragging;

    void Start()
    {
        RequestUserInfo.Instance.CreateNewUser("raul", "aaa");
        // RequestUserInfo.Instance.ModifyUserCritteron("673b1caf2a81f24efe0796c8", "673a1a8c69f25059e2e01e0c", level: 10, currentLife: 89, combatWins: 10, stepAsPartner: 190);

        //RequestUserInfo.Instance.ModifyUserLevel("673a8540bc255e64e48ef7b2", 10);
        isDragging = false;
        moveSpeed /= 100;

        float halfWidth = hotelWidth / 2f;
        float halfHeight = hotelHeight / 2f;

        // Definimos los limites de la pantalla
        minX = -halfWidth;
        maxX = halfWidth;
        minY = -halfHeight;
        maxY = halfHeight;

    }
    void Update()
    {
        // Editor de Unity
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            MoveCameraWithInput(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // En dispositivos android
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                StartDragging(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                MoveCameraWithInput(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }
#endif
    }

    private void StartDragging(Vector3 position)
    {
        lastPos = position;
        isDragging = true;
    }

    private void MoveCameraWithInput(Vector3 currentPosition)
    {
        Vector3 touchDelta = currentPosition - lastPos;
        Vector3 cameraMovement = new Vector3(-touchDelta.x * moveSpeed, -touchDelta.y * moveSpeed, 0);

        moveCam(cameraMovement);
        lastPos = currentPosition;
    }

    private void moveCam(Vector3 vector)
    {
        Vector3 newPosition = transform.position + vector;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;
    }
}
