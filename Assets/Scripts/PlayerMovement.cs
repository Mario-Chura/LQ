using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform playerCamera; // Referencia a la cámara del jugador.
    public float walkSpeed = 3f;
    public float runSpeed = 9f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float mouseSensitivity = 100f; // Sensibilidad del mouse para la rotación.

    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f; // Rotación en el eje X (mirar arriba/abajo).

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Start()
    {
        // Bloquear el cursor en el centro de la pantalla y ocultarlo.
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (PlayerSingleton.isPaused || PlayerSingleton.isGameOver)
        {
            return;
        }

        // Movimiento de la cámara con el ratón.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotar el jugador en el eje Y (izquierda/derecha).
        transform.Rotate(Vector3.up * mouseX);

        // Controlar la rotación en el eje X (mirar arriba/abajo).
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar la rotación para no girar de más.
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Revisar si el jugador está en el suelo.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Asegurar que el personaje no siga cayendo.
        }

        // Obtener el input de movimiento.
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Determinar si el jugador está corriendo.
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Mover al jugador en función de la dirección en la que está mirando.
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Salto.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Aplicar gravedad.
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
