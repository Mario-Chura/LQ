using UnityEngine;

public class AttackState : IEnemyState
{
    private EnemyController controller;

    public AttackState(EnemyController controller)
    {
        this.controller = controller;
    }

    public void EnterState()
    {
        controller.Agent.speed = controller.RunSpeed;
        controller.Animator.SetBool("isWalking", false);
        controller.Animator.SetBool("isRunning", false);
        controller.Animator.SetBool("isAtacking", true);

    }

    public void UpdateState()
    {
        if (controller.Target != null)
        {
            //controller.Agent.SetDestination(controller.Target.position);

            // set no destination
            controller.Agent.SetDestination(controller.transform.position);
        }
    }
}
