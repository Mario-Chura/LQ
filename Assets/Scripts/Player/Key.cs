using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    private bool hasBeenCollected = false;
    
    public string GetInteractionMessage()
    {
        return hasBeenCollected ? "" : "Press E to attempt to collect the key";
    }

    public void Interact()
    {
        if (hasBeenCollected) return;
        
        // Buscar el MathQuestion manager y mostrar la pregunta
        MathQuestion mathQuestion = FindObjectOfType<MathQuestion>();
        if (mathQuestion != null)
        {
            // Pasar referencia de esta llave al sistema de preguntas
            mathQuestion.SetCurrentKey(this);
            mathQuestion.ShowMathQuestion();
        }
        else
        {
            Debug.LogError("MathQuestion not found!");
            // Fallback: recoger directamente
            CollectKeyDirectly();
        }
    }
    
    // Método público para ser llamado cuando se responde correctamente
    public void CollectKeySuccessfully()
    {
        if (hasBeenCollected) return;
        
        hasBeenCollected = true;
        
        // DESTRUIR completamente el objeto y todos sus hijos
        Destroy(transform.root.gameObject);
        
        // NUEVO: Notificar al KeySpawner para spawnar la siguiente llave
        if (KeySpawner.Instance != null)
        {
            KeySpawner.Instance.OnKeyCollected();
        }
        
        Debug.Log("Key collected successfully and destroyed!");
    }
    
    // Método para recoger directamente (fallback)
    private void CollectKeyDirectly()
    {
        PlayerSingleton.Instance.playerInventory.CollectKey();
        CollectKeySuccessfully();
        
        // NUEVO: Notificar al KeySpawner para spawnar la siguiente llave
        if (KeySpawner.Instance != null)
        {
            KeySpawner.Instance.OnKeyCollected();
        }
    }
}