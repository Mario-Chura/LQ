using UnityEngine;
using System.Collections;

public class DoorInteractive : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float openAngle = -90f; // Ángulo final en el eje Y para abrir la puerta
    [SerializeField] private float openSpeed = 2f;   // Velocidad de apertura

    public string GetInteractionMessage()
    {
        if (isOpen)
        {
            return "";
        }
        
        PlayerInventory inventory = PlayerSingleton.Instance.playerInventory;
        
        if (inventory.HasAllKeys())
        {
            return "Press E to open the door";
        }
        else
        {
            int currentKeys = inventory.GetCurrentKeys();
            int requiredKeys = inventory.GetRequiredKeys();
            return $"You need {requiredKeys} keys to open this door. You have {currentKeys}/{requiredKeys}";
        }
    }

    public void Interact()
    {
        if (!isOpen && PlayerSingleton.Instance.playerInventory.HasAllKeys())
        {
            Debug.Log("Door opened with all keys!");
            isOpen = true;
            StartCoroutine(OpenDoor());

            // disable the collider
            GetComponent<Collider>().enabled = false;
        }
        else if (!PlayerSingleton.Instance.playerInventory.HasAllKeys())
        {
            PlayerInventory inventory = PlayerSingleton.Instance.playerInventory;
            int currentKeys = inventory.GetCurrentKeys();
            int requiredKeys = inventory.GetRequiredKeys();
            Debug.Log($"You need all {requiredKeys} keys! You currently have {currentKeys}.");
        }
    }

    private IEnumerator OpenDoor()
    {
        isOpen = true; // Marcar la puerta como abierta
        Transform parent = transform.parent;

        Quaternion startRotation = parent.rotation; // Rotación inicial
        Quaternion targetRotation = Quaternion.Euler(0f, openAngle, 0f); // Rotación final

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            parent.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * openSpeed;
            yield return null;
        }

        parent.rotation = targetRotation; // Asegurar que la rotación final sea precisa
    }
}