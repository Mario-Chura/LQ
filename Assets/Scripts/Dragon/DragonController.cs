using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using static DragonState;

public class DragonController : MonoBehaviour
{


    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator anim;
    DragonState currentState;

    [SerializeField] private Transform player;

    [Tooltip("Para obtener el tamanio del objeto")]
    [SerializeField] private Renderer rend;

    [SerializeField] private GameObject particleFire;

    [SerializeField] public float visDist = 20.0f; //distancia de vision
    [SerializeField] public float visAngle = 270.0f; //vision de angulo
    [SerializeField] public float shootDist = 5f; //rango de ataque
    [SerializeField] public float distOutNavMesh = 1.0f; //radio de "point llegada" en el nav mesh


    private bool boolBrokenBottle; 

    void Start()
    {
        boolBrokenBottle = false;
        #region tamanio objeto
        //Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Vector3 size = rend.bounds.size;
            Debug.Log("Tamaño del objeto en unidades: " + size);
        }
        #endregion
        currentState = new DragonCaminar(gameObject, agent, anim, player, particleFire);
        
        currentState.visDist = visDist;
        currentState.visAngle = visAngle;
        currentState.shootDist = shootDist;
        currentState.distOutNavMesh = distOutNavMesh;

        //empieza apagando el fuego
        gameObject.GetComponentInChildren<ParticleSystem>().Stop();
    }


    void Update()
    {
        currentState = currentState.Process();
    }

    public void cambiarEstadoDragonBuscarSonido(Vector3 brokenBottle)
    {
        //Forzar salida del estado actual
        if (currentState != null)
        {
            currentState.stage = EVENT.EXIT;
            currentState = currentState.Process(); // Procesar la salida
        }

        // Cambiar al estado de búsqueda de botella
        currentState = new DragonBuscarSonido(gameObject, agent, anim, player, brokenBottle, particleFire);

        currentState.visDist = visDist;
        currentState.visAngle = visAngle;
        currentState.shootDist = shootDist;
        currentState.distOutNavMesh = distOutNavMesh;

        Debug.Log("Dragon dirigiéndose a investigar botella rota");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over...");
            
            //se hace en el state
            //currentState = new DragonPegar(gameObject, agent, anim, player);
            //currentState.stage = EVENT.EXIT; //cerramos el anterior estado

            PlayerSingleton.Instance.GameOver(); //se acaba el juego
        }
    }

}
