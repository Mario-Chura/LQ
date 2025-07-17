using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensibilidad del mouse
    public float moveSpeed = 10f;         // Velocidad de movimiento
    public float maxVerticalAngle = 85f; // Ángulo máximo hacia arriba/abajo

    private float pitch = 0f;            // Rotación vertical
    private float yaw = 0f;              // Rotación horizontal

    void Start()
    {
        // Bloquear el cursor en el centro de la pantalla y ocultarlo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Rotación de la cámara con el mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;

        // Restringir la rotación vertical para evitar voltear la cámara
        pitch = Mathf.Clamp(pitch, -maxVerticalAngle, maxVerticalAngle);

        // Aplicar rotación
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        // Movimiento con teclas WASD o flechas
        Vector3 moveDirection = new Vector3(
            Input.GetAxis("Horizontal"), // Movimiento horizontal (A/D o flechas izquierda/derecha)
            0,                           // Movimiento vertical desactivado para mantener en el plano
            Input.GetAxis("Vertical")    // Movimiento hacia adelante/atrás (W/S o flechas arriba/abajo)
        );

        // Permitir movimiento en cualquier dirección
        if (Input.GetKey(KeyCode.Space)) moveDirection.y += 1;   // Ascender (barra espaciadora)
        if (Input.GetKey(KeyCode.LeftShift)) moveDirection.y -= 1; // Descender (Shift)

        // Normalizar dirección y aplicar velocidad
        moveDirection = transform.TransformDirection(moveDirection.normalized) * moveSpeed * Time.deltaTime;

        // Aplicar movimiento
        transform.position += moveDirection;

        // Salir del modo espectador
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}