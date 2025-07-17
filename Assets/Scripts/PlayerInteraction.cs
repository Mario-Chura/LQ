using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera; // Cámara del jugador.
    [SerializeField] private Transform handPosition; // Posición para sostener el objeto.
    [SerializeField] private LineRenderer lineRenderer; // Línea para dibujar la trayectoria.

    [Header("Trajectory Settings")]
    [SerializeField] private int trajectoryPoints = 30; // Cantidad de puntos en la trayectoria.
    [SerializeField] private float maxThrowForce = 20f; // Fuerza máxima de lanzamiento.
    [SerializeField] private float forceIncrement = 10f; // Incremento de fuerza por segundo.
    [SerializeField] private float timeBetweenPoints = 0.1f; // Tiempo entre puntos de la trayectoria.

    private GameObject heldObject = null; // Objeto actualmente sostenido.
    private float currentThrowForce = 0f; // Fuerza actual cargada.
    private bool isCharging = false; // Si se está cargando la fuerza.
    private List<Vector3> trajectoryPositions = new List<Vector3>(); // Puntos de la trayectoria.

    [Header("Key and Door System")]
    public string correctDoorTag = "Door1"; // Tag de la puerta correcta.
    private bool hasKey = false; // Indica si el jugador tiene una llave.

    void Update()
    {
        // Lógica de interacción
        if (Input.GetKeyDown(KeyCode.E)) // Intentar recoger objetos con 'E'
        {
            HandleObjectInteraction();
        }

        // Lógica de lanzamiento
        if (heldObject != null)
        {
            HandleThrowing();
        }
    }

    private void HandleObjectInteraction()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            // Si es un objeto lanzable
            if (hit.collider.CompareTag("Throwable"))
            {
                PickUpObject(hit.collider.gameObject);
            }
            // Si es una llave
            else if (hit.collider.CompareTag("Key"))
            {
                CollectKey(hit.collider.gameObject);
            }
            // Si es una puerta
            else if (hit.collider.CompareTag("Door1"))
            {
                TryOpenDoor(hit.collider.gameObject);
            }
        }
    }

    private void PickUpObject(GameObject obj)
    {
        heldObject = obj.transform.parent.gameObject;
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        heldObject.transform.SetParent(handPosition);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    }

    private void CollectKey(GameObject keyObject)
    {
        hasKey = true;
        Destroy(keyObject);
        Debug.Log("¡Llave recogida! Ahora puedes abrir puertas específicas.");
    }

    private void TryOpenDoor(GameObject doorObject)
    {
        Door door = doorObject.GetComponentInParent<Door>();
        if (door != null)
        {
            door.TryOpenDoor(hasKey, correctDoorTag);
        }
    }

    private void HandleThrowing()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isCharging = true;
            currentThrowForce = 0f;
            lineRenderer.enabled = true;
        }

        if (Input.GetMouseButton(1) && isCharging)
        {
            currentThrowForce += forceIncrement * Time.deltaTime;
            currentThrowForce = Mathf.Clamp(currentThrowForce, 0, maxThrowForce);
            ShowTrajectory();
        }

        if (Input.GetMouseButtonUp(1) && isCharging)
        {
            isCharging = false;
            LaunchHeldObject();
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }
    }

    private void ShowTrajectory()
    {
        trajectoryPositions.Clear();
        Vector3 startPoint = handPosition.position;
        Vector3 startVelocity = playerCamera.forward * currentThrowForce;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * timeBetweenPoints;
            Vector3 point = CalculatePositionAtTime(startPoint, startVelocity, time);
            trajectoryPositions.Add(point);

            if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, 0.1f)) break;
        }

        lineRenderer.positionCount = trajectoryPositions.Count;
        lineRenderer.SetPositions(trajectoryPositions.ToArray());
    }

    private Vector3 CalculatePositionAtTime(Vector3 startPosition, Vector3 initialVelocity, float time)
    {
        Vector3 gravity = Physics.gravity;
        return startPosition + initialVelocity * time + 0.5f * gravity * time * time;
    }

    private void LaunchHeldObject()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.velocity = playerCamera.forward * currentThrowForce;
            }

            heldObject.transform.SetParent(null);
            heldObject = null;
            currentThrowForce = 0f;
        }
    }

    public bool HasKey()
    {
        return hasKey;
    }
}
