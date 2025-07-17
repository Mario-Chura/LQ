using UnityEngine.AI;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float WalkSpeed = 2f;
    public float RunSpeed = 5f;
    public Transform[] Waypoints;
    public float StopDistance = 0.5f;

    [Header("Detection Settings")]
    public float LoseSightTime = 10f;

    [Header("References")]
    public Transform Target;
    public NavMeshAgent Agent;
    public Animator Animator;
    public EnemyVision Vision;
    public SoundListener soundListener;

    private IEnemyState currentState;
    public PatrolState patrolState;
    public ChaseState chaseState;
    public AttackState attackState;
    public SoundInvestigationState soundInvestigationState;

    public Vector3 lastKnownPlayerPosition;
    private float timeSinceLastSeen;
    private bool isAttacking = false;

    void Start()
    {
        if (Agent == null) Agent = GetComponent<NavMeshAgent>();
        if (Animator == null) Animator = GetComponent<Animator>();
        if (Vision == null) Vision = GetComponent<EnemyVision>();

        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        soundInvestigationState = new SoundInvestigationState(this);

        SwitchState(patrolState);
        soundListener.SetHearingCallback(OnListenSound);
    }

    void Update()
    {
        currentState.UpdateState();

        if (isAttacking)
        {
            SwitchState(attackState);
        }
        else if (Vision.IsPlayerInSight(Target))
        {
            lastKnownPlayerPosition = Target.position;
            timeSinceLastSeen = 0f;
            SwitchState(chaseState);
        }
        else if (currentState == chaseState)
        {
            timeSinceLastSeen += Time.deltaTime;
            if (timeSinceLastSeen >= LoseSightTime)
            {
                SwitchState(patrolState);
            }
        }
    }

    public void OnListenSound(SoundData soundData, float intensity)
    {
        if (intensity > 0.6f && !soundData.soundType.StartsWith("enemy"))
        {
            Debug.Log("Heard sound: " + soundData.soundType + " at " + soundData.position + " with intensity " + intensity);

            if (currentState == soundInvestigationState && soundInvestigationState.investigationTimer > 0f)
            {
                Debug.Log("Already investigating sound");
                return;
            }

            if (currentState == chaseState && soundData.soundType.StartsWith("player"))
            {
                Debug.Log("Heard player sound while chasing");
                return;
            }

            soundInvestigationState.SetSoundPosition(soundData.position);
            SwitchState(soundInvestigationState);
        }
    }

    public void SwitchState(IEnemyState newState)
    {
        currentState = newState;
        currentState.EnterState();
    }

    public void PauseAnimations()
    {
        Animator.speed = 0;
    }

    public void ResumeAnimations()
    {
        Animator.speed = 1;
    }

    // on trigger set attacking and game over
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy Trigger " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over");
            PlayerSingleton.Instance.GameOver();
            isAttacking = true;
        }
    }
}
