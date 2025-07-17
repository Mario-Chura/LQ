using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IEnemyState
{
    private EnemyController controller;
    private int currentWaypointIndex = 0;

    private float timeChangeWaypoint = 0f;

    public PatrolState(EnemyController controller)
    {
        this.controller = controller;
    }

    public void EnterState()
    {
        controller.Agent.speed = controller.WalkSpeed;
        controller.Animator.SetBool("isWalking", true);
        controller.Animator.SetBool("isRunning", false);
    }

    public void UpdateState()
    {
        if (controller.Waypoints.Length == 0) return;

        float distanceWaypoint = Vector3.Distance(controller.transform.position, controller.Waypoints[currentWaypointIndex].position);


        // change waypoint every 10 seconds or if is in waypoint position
        timeChangeWaypoint += Time.deltaTime;
        if (timeChangeWaypoint >= 10f || distanceWaypoint < controller.StopDistance)
        {
            Debug.Log("Change waypoint");
            //currentWaypointIndex = (currentWaypointIndex + 1) % controller.Waypoints.Length;

            // random waypoint
            currentWaypointIndex = Random.Range(0, controller.Waypoints.Length);

            timeChangeWaypoint = 0f;
        }

        controller.Agent.SetDestination(controller.Waypoints[currentWaypointIndex].position);

        //if (distanceWaypoint < controller.StopDistance - 0.1)
        //{
        //    currentWaypointIndex = (currentWaypointIndex + 1) % controller.Waypoints.Length;
        //}

        
    }
}
