using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Transform eyes;
    public float detectionRange = 60f;
    public float fieldOfViewAngle = 45f;

    public bool IsPlayerInSight(Transform target)
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

    void OnDrawGizmos()
    {
        if (eyes != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 forward = eyes.forward * detectionRange;

            Quaternion leftRayRotation = Quaternion.Euler(0, -fieldOfViewAngle, 0);
            Vector3 leftRayDirection = leftRayRotation * forward;

            Quaternion rightRayRotation = Quaternion.Euler(0, fieldOfViewAngle, 0);
            Vector3 rightRayDirection = rightRayRotation * forward;

            Gizmos.DrawRay(eyes.position, forward);
            Gizmos.DrawRay(eyes.position, leftRayDirection);
            Gizmos.DrawRay(eyes.position, rightRayDirection);
        }
    }
}
