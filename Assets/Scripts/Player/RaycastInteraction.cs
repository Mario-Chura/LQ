using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RaycastInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float rayDistance = 3f;

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI interactionText;

    private IInteractable currentInteractable;

    void Update()
    {
        HandleRaycast();
    }

    private void HandleRaycast()
    {
        interactionText.text = "";
        currentInteractable = null;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            currentInteractable = hit.collider.GetComponent<IInteractable>();
            if (currentInteractable != null)
            {
                Debug.Log("Interactable object found " + currentInteractable.GetInteractionMessage());
                interactionText.text = currentInteractable.GetInteractionMessage();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    currentInteractable.Interact();

                    if (exercisePanel != null && hit.collider.CompareTag("Key"))
                    {

                        //Pausar juego
                        PlayerSingleton.isPaused = true;
                        Time.timeScale = 0.0f; //para pausar el juego

                        exercisePanel.SetActive(true);
                        ShowRandomExercise();

                        //Cursor.lockState = CursorLockMode.None;
                        //Cursor.visible = true;
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerCamera.position, playerCamera.position + playerCamera.forward * rayDistance);
        }
    }


    //  FUNCIONES DE EJERCICIO (AÑADIDAS)
   
    [Header("Exercise Settings")]
    public List<Sprite> exerciseImages;
    public Image displayImage;
    public List<string> correctAnswers;
    public TMP_InputField userInput;
    public GameObject exercisePanel;

    private int currentExerciseIndex = -1;


    [Tooltip("TextMeshProUGUI para colocar si la respuesta es correcta o no")]
    [SerializeField] AudioSource audioSourceAnswer;
    [SerializeField] AudioClip audioAnswerCorrect;
    [SerializeField] AudioClip audioAnswerIncorrect;

    public void ShowRandomExercise()
    {
        if (exerciseImages.Count == 0) return;

        currentExerciseIndex = Random.Range(0, exerciseImages.Count);
        displayImage.sprite = exerciseImages[currentExerciseIndex];
        userInput.text = ""; // Limpiar el campo de respuesta
    }

    public void CheckAnswer()
    {
        if (currentExerciseIndex == -1) return;

        string correct = correctAnswers[currentExerciseIndex];
        string userResponse = userInput.text.Trim();
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (userResponse.Equals(correct.Trim(), System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("Respuesta Correcta");
            exercisePanel.SetActive(false);

            //Pausar juego
            PlayerSingleton.isPaused = false;
            Time.timeScale = 1.0f; //para reanudar el juego

            //agregamos la llave al contador
            //PlayerSingleton.Instance.playerInventory.CollectKey();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //SceneManager.LoadScene(nextSceneIndex);

            //manda mensaje si la respuesta es correcta
            if (audioSourceAnswer != null)
                audioSourceAnswer.PlayOneShot(audioAnswerCorrect);
        }
        else
        {
            Debug.Log("Respuesta Incorrecta. Intenta con otro ejercicio.");
            ShowRandomExercise();

            //manda mensaje si la respuesta es incorrecta
            if (audioSourceAnswer != null)
                audioSourceAnswer.PlayOneShot(audioAnswerIncorrect);
        }
    }
}
