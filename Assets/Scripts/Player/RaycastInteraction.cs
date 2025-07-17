using UnityEngine;
using UnityEngine.UI;


// TMP
using TMPro;


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
                }
            }
        }
    }

    // Dibuja una línea de depuración en la escena
    private void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerCamera.position, playerCamera.position + playerCamera.forward * rayDistance);
        }
        
    }
}
