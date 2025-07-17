using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrajectory : MonoBehaviour
{
    [SerializeField] private Transform launchPoint; // Punto desde donde se lanzará el objeto
    [SerializeField] private GameObject projectilePrefab; // Prefab del objeto que será lanzado
    [SerializeField] private LineRenderer lineRenderer; // LineRenderer para dibujar la trayectoria

    [SerializeField] private int trajectoryPoints = 30; // Cantidad de puntos en la línea de trayectoria
    [SerializeField] private float launchForce = 10f; // Fuerza inicial de lanzamiento
    [SerializeField] private float maxLaunchForce = 20f; // Fuerza máxima de lanzamiento
    [SerializeField] private float forceIncrement = 2f; // Incremento de fuerza por segundo
    [SerializeField] private float timeBetweenPoints = 0.1f; // Tiempo entre cada punto en la línea

    private List<Vector3> trajectoryPositions = new List<Vector3>();
    private float currentLaunchForce = 0f; // Fuerza actual que se está cargando
    private bool isCharging = false; // Si se está cargando el lanzamiento

    void Update()
    {
        // Detectar si se presiona el botón derecho del ratón
        if (Input.GetMouseButtonDown(1)) // Botón derecho
        {
            isCharging = true;
            currentLaunchForce = 0f; // Reiniciar la fuerza al cargar
        }

        // Cargar la fuerza mientras se mantiene presionado el botón derecho
        if (isCharging)
        {
            currentLaunchForce += forceIncrement * Time.deltaTime;
            currentLaunchForce = Mathf.Clamp(currentLaunchForce, 0f, maxLaunchForce);
            ShowTrajectory();
        }

        // Lanzar el proyectil al soltar el botón
        if (Input.GetMouseButtonUp(1)) // Botón derecho
        {
            isCharging = false;
            LaunchProjectile();
            lineRenderer.positionCount = 0; // Limpiar la línea después de lanzar
        }
    }

    void ShowTrajectory()
    {
        trajectoryPositions.Clear();
        Vector3 startPoint = launchPoint.position;
        Vector3 startVelocity = launchPoint.forward * currentLaunchForce;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * timeBetweenPoints;
            Vector3 point = CalculatePositionAtTime(startPoint, startVelocity, time);
            trajectoryPositions.Add(point);

            // Detener el cálculo si el punto toca el suelo
            if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, 0.1f))
            {
                break;
            }
        }

        lineRenderer.positionCount = trajectoryPositions.Count;
        lineRenderer.SetPositions(trajectoryPositions.ToArray());
    }

    Vector3 CalculatePositionAtTime(Vector3 startPosition, Vector3 initialVelocity, float time)
    {
        Vector3 gravity = Physics.gravity;
        return startPosition + initialVelocity * time + 0.5f * gravity * time * time;
    }

    void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = launchPoint.forward * currentLaunchForce; // Aplicar la fuerza al proyectil
    }
}

