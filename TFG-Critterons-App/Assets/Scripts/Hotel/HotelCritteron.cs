
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class HotelCritteron : MonoBehaviour
{
    //metemos todo esto en una clase static config?
    static float updateBehaviourRate = 5f, updateBehaviourRandom = .2f,
        useItemsProbability = 0.7f;
    //si va a cualquie tipo de objeto
    static bool randomObjects = true;

    [SerializeField]
    I_Critteron infoCritteron;

    [SerializeField]
    Transform visualRoot;

    [SerializeField]
    NavMeshAgent agent;

    Animator animator;

    RoomInfo currentRoom;
    HotelObject target;


    public I_Critteron InfoCritteron
    {
        get
        {
            return infoCritteron;
        }
    }

    public RoomInfo CurrentRoom
    {
        get
        {
            return currentRoom;
        }
        set
        {
            currentRoom = value;
        }
    }

    public HotelObject Target
    {
        get
        {
            return target;
        }
    }

  

    public void Start()
    {
        animator.Play("Move");

        InvokeRepeating("BehaviourUpdate", Random.Range(0, updateBehaviourRandom), updateBehaviourRate);
    }

    //animato etc


    public void InitialiceCritteron(I_Critteron info, RoomInfo currentRoom)

    {
        infoCritteron = info;

        gameObject.name = infoCritteron.name;

        var visual = visualRoot.Find(infoCritteron.mesh);

        visual.gameObject.SetActive(true);
        animator = visual.transform.GetComponent<Animator>();

        this.currentRoom = currentRoom;
    }

    public void ForceUpdate(float time)
    {
        Invoke("BehaviourUpdate", time);
    }
    public void StopCritteron()
    {
        agent.enabled = false;

        animator.Play("Idle");
    }

    public void ActivateCritteron(float time = 0)
    {
        target = null;

        if (time == 0)
        {
            agent.enabled = true;
            animator.Play("Move");
        }
        //esta bien u otro metodo?
        else
            StartCoroutine(DelayFunction(() =>
            {
                agent.enabled = true; animator.Play("Move");
            }, time));

    }

    public void ChangePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        agent.Warp(newPosition);
    }

    void BehaviourUpdate()
    {
        //ahora eta en un objeto y su agente desactivado
        if (!agent.enabled)
            return;

        //si ahora mismo no quiere ir a ningunn objeto o si esta en algun objeto actualmente
        if (target == null)
        {
            //si tiene en cuenta el tipo de objeto
            if(!randomObjects)
            {

                //aqui añadiriamos todos los tipos de objeto necesarios

                //aqui seria current live
                if (infoCritteron.life < infoCritteron.life)
                {
                    //intenta ir a ua zona de curacion acelerada
                    target = NavigationControl.GetTarget(HotelObjectType.cureObject);
                }

                else if (target == null)
                {
                    //intenta ir a zona de mejora de nivel acelerada
                    target = NavigationControl.GetTarget(HotelObjectType.levelObject);
                }

                if (Random.Range(0, 1f) <= useItemsProbability)
                {
                    //intenta ir a jugar a algun objeto
                    target = NavigationControl.GetTarget(HotelObjectType.decorationObject);
                }
            }

            //si no lo tiene
            else
            {
                float prob = Random.Range(0, 1f);
                if (prob <= useItemsProbability)
                {
                    //intenta ir a jugar a algun objeto
                    target = NavigationControl.GetTarget();
                }
            }

            if (target != null)
                target.CurrentUser = this;

        }


        if (target != null)
        {
            Debug.Log(gameObject.name + " moviendose a " + Target.gameObject.name);

            //esta yendo a algun kugar
            agent.SetDestination(NavigationControl.GetNextRoutePoint(this).position);
        }

        else
        {
            Debug.Log(gameObject.name + " moviendose a posicion aleatoria");

            //sitio aleatorio de la habitacion
            agent.SetDestination(NavigationControl.GetRandomPoint(this));
        }





    }

    IEnumerator DelayFunction(UnityAction action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }




}
