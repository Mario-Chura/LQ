using UnityEngine;

public class SoundInvestigationState : IEnemyState
{
    private EnemyController controller;
    private Vector3 soundPosition;
    public float investigationTimer;
    private const float InvestigationTime = 5f;

    public SoundInvestigationState(EnemyController controller)
    {
        this.controller = controller;
    }

    public void EnterState()
    {

        controller.Agent.speed = controller.WalkSpeed;
        controller.Animator.SetBool("isWalking", true);
        controller.Animator.SetBool("isRunning", false);

        investigationTimer = InvestigationTime;
        controller.Agent.SetDestination(soundPosition);
    }

    public void SetSoundPosition(Vector3 position)
    {
        soundPosition = position;
    }

    public void UpdateState()
    {
        if (Vector3.Distance(controller.transform.position, soundPosition) < controller.StopDistance)
        {
            investigationTimer -= Time.deltaTime;
            if (investigationTimer <= 0f)
            {
                controller.SwitchState(controller.patrolState);
            }
        }
        else
        {
            controller.Agent.SetDestination(soundPosition);
        }
    }
}
