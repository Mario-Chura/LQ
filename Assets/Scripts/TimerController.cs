using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timeRemaining = 60f;
    public Text timerText;
    public GameObject gameOverPanel;

    private bool isGameOver = false;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip clockPickupSound;

    private Rigidbody rb; // Para bloquear movimiento
    private CharacterController characterController;

    void Start()
    {
        UpdateTimerDisplay();
        gameOverPanel.SetActive(false);

        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!isGameOver)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                TriggerGameOver();
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = "Tiempo: " + seconds.ToString();
    }

    private void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Bloquear movimiento si tiene Rigidbody o CharacterController
        if (rb != null) rb.isKinematic = true;
        if (characterController != null) characterController.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return;

        if (other.CompareTag("Reloj"))
        {
            timeRemaining += 30f;
            if (audioSource != null && clockPickupSound != null)
            {
                audioSource.PlayOneShot(clockPickupSound);
            }
            Destroy(other.gameObject);
            UpdateTimerDisplay();
        }
        else if (other.CompareTag("Lobo"))
        {
            TriggerGameOver();
        }
    }
}
