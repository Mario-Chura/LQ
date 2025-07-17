using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    public float stopDistance = 0.5f;

    [Header("Detection Settings")]
    public Transform eyes; // Punto de origen del cono de visión
    public float detectionRange = 60f;
    public float fieldOfViewAngle = 45f; // Ángulo del cono de visión
    public float loseSightTime = 10f; // Tiempo para perder el contacto visual

    [Header("References")]
    public Transform target;
    private Animator animator;
    public TMPro.TextMeshProUGUI gameOverText;
    public NavMeshAgent agent;

    private float timeSinceLastSeen = 0f; // Temporizador para perder de vista al jugador
    private bool isChasing = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        agent.speed = walkSpeed;

        // Ajusta los waypoints para que mantengan la altura del enemigo
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 position = waypoints[i].position;
            position.y = transform.position.y;
            waypoints[i].position = position;
        }
    }

    void Update()
    {
        if (IsPlayerInSight())
        {
            // Reinicia el temporizador cuando el jugador está a la vista
            timeSinceLastSeen = 0f;
            isChasing = true;
            PursuePlayer();
        }
        else if (isChasing)
        {
            // Incrementa el temporizador si se pierde el contacto visual
            timeSinceLastSeen += Time.deltaTime;

            if (timeSinceLastSeen >= loseSightTime)
            {
                // Deja de perseguir después de perder el contacto visual por 10 segundos
                isChasing = false;
                Patrol();
            }
            else
            {
                // Continúa persiguiendo durante el tiempo de gracia
                PursuePlayer();
            }
        }
        else
        {
            Patrol();
        }
    }

    // Método para patrullar entre waypoints
    void Patrol()
    {
        agent.speed = walkSpeed;
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);

        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentWaypointIndex].position);

        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < stopDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // Método para perseguir al jugador
    void PursuePlayer()
    {
        agent.speed = runSpeed;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);

        agent.SetDestination(target.position);
    }

    // Método para detectar si el jugador está en el cono de visión
    bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = target.position - eyes.position;
        float angleToPlayer = Vector3.Angle(directionToPlayer, eyes.forward);

        if (angleToPlayer < fieldOfViewAngle)
        {
            RaycastHit hit;
            if (Physics.Raycast(eyes.position, directionToPlayer.normalized, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Visualización del cono de visión con Gizmos
    void OnDrawGizmos()
    {
        if (eyes != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 forward = eyes.forward * detectionRange;

            // Límite izquierdo del cono
            Quaternion leftRayRotation = Quaternion.Euler(0, -fieldOfViewAngle, 0);
            Vector3 leftRayDirection = leftRayRotation * forward;

            // Límite derecho del cono
            Quaternion rightRayRotation = Quaternion.Euler(0, fieldOfViewAngle, 0);
            Vector3 rightRayDirection = rightRayRotation * forward;

            // Dibujar el cono de visión
            Gizmos.DrawRay(eyes.position, forward);
            Gizmos.DrawRay(eyes.position, leftRayDirection);
            Gizmos.DrawRay(eyes.position, rightRayDirection);

            // Dibujar una línea hasta el jugador si está dentro del cono
            if (IsPlayerInSight())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(eyes.position, target.position);
            }
        }
    }

    // Lógica de colisión con el jugador
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameOverText.gameObject.SetActive(true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isAtacking", true);
            Time.timeScale = 0;
            Debug.Log("Juego Terminado");
        }
    }
}
