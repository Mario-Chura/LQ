using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsEnableDisableCanvas : MonoBehaviour
{
    public static ObjectsEnableDisableCanvas Instance { get; private set; }

    [Tooltip("Para desactivar la mira y el raycast")]
    [SerializeField] GameObject objectSight;

    [Tooltip("Desactivar objeto de texto E")]
    [SerializeField] GameObject objectTextE;

    [Tooltip("Desactivar contador de llaves obtenidas")]
    [SerializeField] GameObject objectCountKeys;

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

    public void ActiveObjectsCanvasMathQuestion()
    {
        if(objectTextE != null)
            objectTextE.SetActive(true);
        if(objectSight != null)
            objectSight.SetActive(true);
        if(objectCountKeys != null)
            objectCountKeys.SetActive(true);
    }

    public void DesactiveObjectsCanvasMathQuestion()
    {
        if (objectTextE != null)
            objectTextE.SetActive(false);
        if (objectSight != null)
            objectSight.SetActive(false);
        if (objectCountKeys != null)
            objectCountKeys.SetActive(false);
    }
}
