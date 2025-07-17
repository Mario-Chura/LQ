using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    [SerializeField] private GameObject keyPrefab; // Prefab de la llave.
    [SerializeField] private Transform[] keySpawnPoints; // Puntos de aparición de la llave.
    [SerializeField] private int totalKeysRequired = 3; // Número total de llaves en el juego
    
    private int currentKeyIndex = 0; // Índice de la llave actual (0, 1, 2)
    private List<int> usedSpawnPoints = new List<int>(); // Puntos ya usados
    
    public static KeySpawner Instance; // Singleton para acceso fácil

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnFirstKey();
    }

    private void SpawnFirstKey()
    {
        currentKeyIndex = 0;
        usedSpawnPoints.Clear();
        SpawnNextKey();
    }

    public void SpawnNextKey()
    {
        // Verificar si ya se spawnearon todas las llaves
        if (currentKeyIndex >= totalKeysRequired)
        {
            Debug.Log("¡Todas las llaves han sido spawneadas!");
            return;
        }

        // Verificar que tengamos suficientes puntos de spawn
        if (keySpawnPoints.Length == 0)
        {
            Debug.LogError("No hay puntos de spawn asignados!");
            return;
        }

        // Obtener un punto de spawn aleatorio que no se haya usado
        int spawnPointIndex = GetRandomUnusedSpawnPoint();
        
        if (spawnPointIndex == -1)
        {
            Debug.LogWarning("No hay más puntos de spawn disponibles, reutilizando puntos...");
            usedSpawnPoints.Clear(); // Resetear puntos usados
            spawnPointIndex = Random.Range(0, keySpawnPoints.Length);
        }

        // Spawnar la llave
        GameObject spawnedKey = Instantiate(keyPrefab, keySpawnPoints[spawnPointIndex].position, Quaternion.identity);
        spawnedKey.name = $"Key_{currentKeyIndex + 1}";
        
        // Marcar el punto como usado
        usedSpawnPoints.Add(spawnPointIndex);
        
        Debug.Log($"Key {currentKeyIndex + 1}/{totalKeysRequired} spawned at position: {keySpawnPoints[spawnPointIndex].name}");
        
        currentKeyIndex++;
    }

    private int GetRandomUnusedSpawnPoint()
    {
        // Crear lista de índices disponibles
        List<int> availableIndices = new List<int>();
        
        for (int i = 0; i < keySpawnPoints.Length; i++)
        {
            if (!usedSpawnPoints.Contains(i))
            {
                availableIndices.Add(i);
            }
        }
        
        // Si no hay puntos disponibles, retornar -1
        if (availableIndices.Count == 0)
        {
            return -1;
        }
        
        // Retornar un índice aleatorio de los disponibles
        return availableIndices[Random.Range(0, availableIndices.Count)];
    }

    // Método llamado cuando se recoge una llave
    public void OnKeyCollected()
    {
        PlayerInventory inventory = PlayerSingleton.Instance.playerInventory;
        
        if (inventory.GetCurrentKeys() < totalKeysRequired)
        {
            Debug.Log($"Key collected! Spawning next key... ({inventory.GetCurrentKeys()}/{totalKeysRequired})");
            SpawnNextKey();
        }
        else
        {
            Debug.Log("¡Todas las llaves han sido recolectadas!");
        }
    }

    // Método para reiniciar el sistema (útil para restart)
    public void ResetKeySystem()
    {
        // Destruir todas las llaves existentes
        GameObject[] existingKeys = GameObject.FindGameObjectsWithTag("Key");
        foreach (GameObject key in existingKeys)
        {
            Destroy(key);
        }
        
        // Reiniciar el sistema
        SpawnFirstKey();
    }

    // Visualizar los puntos de spawn en el editor
    private void OnDrawGizmos()
    {
        if (keySpawnPoints == null) return;
        
        for (int i = 0; i < keySpawnPoints.Length; i++)
        {
            if (keySpawnPoints[i] != null)
            {
                // Color diferente para puntos usados vs disponibles
                Gizmos.color = usedSpawnPoints.Contains(i) ? Color.red : Color.yellow;
                Gizmos.DrawSphere(keySpawnPoints[i].position, 0.5f);
                
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(keySpawnPoints[i].position + Vector3.up, $"Spawn {i + 1}");
                #endif
            }
        }
    }
}