using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckpointsSingleton : MonoBehaviour
{

    public GameObject [] checkpointsDragon;

    public static CheckpointsSingleton Instance { get; private set; }

    void Awake()
    {
        // Implementacion del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantiene el GameManager entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }
}