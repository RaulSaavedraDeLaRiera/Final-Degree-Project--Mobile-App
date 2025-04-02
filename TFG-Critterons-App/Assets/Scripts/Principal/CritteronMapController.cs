using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public class CritteronMapController : MonoBehaviour
{
    Animator animator;
    [SerializeField]
    Transform visualRoot;
    [SerializeField]
    GameObject plataform;
    [SerializeField]
    bool lookAtCamera;

    void Start()
    {
        RequestGameInfo.Instance.GetCritteronByID(PlayerPrefs.GetString("CurrentCritteronID"), critteron =>
        {
            var visual = visualRoot.Find(critteron.name);

            if (visual != null)
            {
                animator = visual.GetComponent<Animator>();
                visual.gameObject.SetActive(true);
            }
        });


    }

    // Update is called once per frame
    void Update()
    {
        if (plataform != null && lookAtCamera)
        {
            Vector3 direction = Camera.main.transform.position - transform.position;
            direction.y = 0; // Mantiene el objeto recto
            plataform.transform.rotation = Quaternion.LookRotation(direction);

        }
    }
}
