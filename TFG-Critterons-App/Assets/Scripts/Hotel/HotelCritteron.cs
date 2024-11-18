using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class HotelCritteron : MonoBehaviour
{
    //metemos todo esto en una clase static config?
    static float updateBehaviourRate = 5f, updateBehaviourRandom = .2f,
        useItemsProbability = 0.1f,
        timeInLevelObjects = 20, randomTimeInLevelObjects = 3,
        timeInDecorationObjects = 20, randomTimeInDecorationObjects = 3;

    [SerializeField]
    I_Critteron infoCritteron;

    [SerializeField]
    Transform visualRoot;

    [SerializeField]
    NavMeshAgent agent;

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
    }


    public void Start()
    {
        InvokeRepeating("BehaviourUpdate", Random.Range(0, updateBehaviourRandom), updateBehaviourRate);
    }

    //animato etc


    public void InitialiceCritteron(I_Critteron info, RoomInfo currentRoom)

    {
        infoCritteron = info;

        gameObject.name = infoCritteron.name;

        var visual = visualRoot.Find(infoCritteron.mesh);

        visual.gameObject.SetActive(true);

        this.currentRoom = currentRoom;
    }


    void BehaviourUpdate()
    {
        //return desde que cuele alguna


        //ahora eta en un objeto y su agente desactivado
        if (!agent.enabled)
            return;

        //si ahora mismo no quiere ir a ningunn objeto o si esta en algun objeto actualmente
        if(target == null)
        {
            //aqui seria current live
            if (3 < infoCritteron.life)
            {
                //intenta ir a ua zona de curacion acelerada
                target = NavigationControl.GetTarget(HotelObjectType.cureObject);

                if(target != null)
                {
                    //va a el
                    return;
                }
            }

            else
            {
                //intenta ir a zona de mejora de nivel acelerada
                target = NavigationControl.GetTarget(HotelObjectType.levelObject);

                if (target != null)
                {
                    //va a el
                    return;
                }
            }

            if (Random.Range(0, 1f) <= useItemsProbability)
            {
                //intenta ir a jugar a algun objeto
                target = NavigationControl.GetTarget(HotelObjectType.decorationObject);

                if (target != null)
                {
                    //va a el
                    return;
                }
            }

            //se mueve a sitio aleatorio de la habitacion
            agent.SetDestination(NavigationControl.GetRandomPoint(this));

        }

        else
        {
            //esta yendo a algun kugar
        }



        
    }




}
