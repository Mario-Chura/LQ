using UnityEngine;

public class ChaseState : IEnemyState
{
    private EnemyController controller;

    public ChaseState(EnemyController controller)
    {
        this.controller = controller;
    }

    public void EnterState()
    {
        controller.Agent.speed = controller.RunSpeed;
        controller.Animator.SetBool("isRunning", true);
        controller.Animator.SetBool("isWalking", false);
    }

    public void UpdateState()
    {
        if (controller.Vision.IsPlayerInSight(controller.Target))
        {
            controller.Agent.SetDestination(controller.Target.position);
        }
        else
        {
            controller.Agent.SetDestination(controller.lastKnownPlayerPosition);
        }
    }
}
