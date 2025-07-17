using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorCanvas : MonoBehaviour
{
    void Update()
    {
        // Detectar la tecla P
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(!PlayerSingleton.isGameOver){
                if (PlayerSingleton.isPaused)
                {
                    PlayerSingleton.Instance.ResumeGame(); // Si está pausado, reanudar
                }
                else
                {
                    PlayerSingleton.Instance.PauseGame(); // Si no está pausado, pausar
                }
            }            
        }

        //Solo para Pruebas
        /*if (Input.GetKeyDown(KeyCode.A))
        {
            TerminarJuego();
        }*/ 
    }

}
