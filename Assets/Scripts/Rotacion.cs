using UnityEngine;

public class Rotacion : MonoBehaviour
{
    public float rotationSpeed = 50f; // Grados por segundo

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
