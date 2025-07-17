using System.Collections.Generic;
using UnityEngine;

public class ObjectThrowing : MonoBehaviour
{
    [Header("Throwing Settings")]
    [SerializeField] public Transform handPosition;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int trajectoryPoints = 30;
    [SerializeField] private float maxThrowForce = 20f;
    [SerializeField] private float forceIncrement = 10f;
    [SerializeField] private float timeBetweenPoints = 0.1f;

    private ThrowableObject heldObject;
    private float currentThrowForce = 0f;
    private bool isCharging = false;
    private List<Vector3> trajectoryPositions = new List<Vector3>();

    void Update()
    {
        if (heldObject != null)
        {
            HandleThrowing();
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
            heldObject.Throw(playerCamera.forward * currentThrowForce);
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
            heldObject = null;
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
            Vector3 point = startPoint + startVelocity * time + 0.5f * Physics.gravity * time * time;
            trajectoryPositions.Add(point);
        }

        lineRenderer.positionCount = trajectoryPositions.Count;
        lineRenderer.SetPositions(trajectoryPositions.ToArray());
    }

    public void SetHeldObject(ThrowableObject obj)
    {
        heldObject = obj;
    }
}
