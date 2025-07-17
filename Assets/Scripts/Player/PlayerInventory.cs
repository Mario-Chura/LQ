using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [Header("Key System")]
    public int totalKeysRequired = 3; // Número total de llaves necesarias
    private int currentKeys = 0; // Llaves actuales recolectadas
    
    [Header("UI References")]
    public TextMeshProUGUI keyCounterText; // Texto para mostrar contador (opcional)

    public void CollectKey()
    {
        currentKeys++;
        Debug.Log($"Key collected! Keys: {currentKeys}/{totalKeysRequired}");
        
        // Actualizar UI si está asignado
        UpdateKeyCounterUI();
        
        // Verificar si ya tiene todas las llaves
        if (HasAllKeys())
        {
            Debug.Log("¡Tienes todas las llaves! Ahora puedes abrir la puerta.");
        }
        else
        {
            Debug.Log($"Necesitas {totalKeysRequired - currentKeys} llaves más.");
        }
    }

    public bool HasKey()
    {
        // Mantener compatibilidad con el sistema anterior (para una sola llave)
        return currentKeys > 0;
    }
    
    public bool HasAllKeys()
    {
        return currentKeys >= totalKeysRequired;
    }
    
    public int GetCurrentKeys()
    {
        return currentKeys;
    }
    
    public int GetRequiredKeys()
    {
        return totalKeysRequired;
    }
    
    private void UpdateKeyCounterUI()
    {
        if (keyCounterText != null)
        {
            keyCounterText.text = $"Llaves: {currentKeys}/{totalKeysRequired}";
        }
    }
    
    // Método para reiniciar las llaves (útil para reiniciar nivel)
    public void ResetKeys()
    {
        currentKeys = 0;
        UpdateKeyCounterUI();
    }
}