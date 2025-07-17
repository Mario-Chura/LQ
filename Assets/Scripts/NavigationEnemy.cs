using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationEnemy : MonoBehaviour
{
    Transform target;
    public Transform main;
    public NavMeshAgent agent;
    public bool isActive= false;
    private float minDistanceToMain = 5.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(isActive){
            agent.SetDestination(target.position);
        } else{
            float distanceToMain = Vector3.Distance(transform.position, main.position);
            if (distanceToMain < minDistanceToMain){
                agent.enabled= false;
            }else{
                agent.enabled= true;
            }
            agent.SetDestination(main.position);
        }
    }

    public void SetBoolIsActive(bool isBoolActive){
        isActive = isBoolActive;
    }

    public void SetTarget(Transform target){
        this.target = target;
    }
}
