using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static DragonState;

public class BottleSingleton : MonoBehaviour
{
    public static BottleSingleton Instance;

    [SerializeField] GameObject[] enemiesDragon;
    [SerializeField] GameObject[] enemiesWolf;

    [SerializeField] float distanceRequired = 30.0f;

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

    //por ahora esta buscando la botella
    public void enemiesSearchSound(Vector3 bottle)
    {

        if (enemiesDragon != null && enemiesDragon.Length > 0)
        {
            for (int i = 0; i < enemiesDragon.Length; i++)
            {
                if (enemiesDragon[i] != null)
                {
                    float distance = Vector3.Distance(bottle, enemiesDragon[i].transform.position);
                    if (distance < distanceRequired)
                    {
                        DragonController estadoComponent = enemiesDragon[i].GetComponent<DragonController>();
                        if (estadoComponent != null)
                        {
                            estadoComponent.cambiarEstadoDragonBuscarSonido(bottle);
                        }
                    }
                }
            }
        }

        if (enemiesWolf != null && enemiesWolf.Length > 0)
        {
            for (int i = 0; i < enemiesWolf.Length; i++)
            {
                if (enemiesWolf[i] != null)
                {
                    float distance = Vector3.Distance(bottle, enemiesWolf[i].transform.position);
                    if (distance < distanceRequired)
                    {
                        EnemyController estadoComponent = enemiesWolf[i].GetComponent<EnemyController>();
                        if (estadoComponent != null)
                        {
                            estadoComponent.SwitchState(estadoComponent.soundInvestigationState);
                            estadoComponent.soundInvestigationState.SetSoundPosition(bottle);
                        }
                    }
                }
            }
        }
    }
}
