using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorPantalla : MonoBehaviour
{
    [SerializeField]
    GameObject pantalla1, pantalla2;

    [SerializeField]
    GameObject[] niveles;

    [SerializeField]
    int nivel;

    void Start(){
       DesbloquearNiveles();
    }

    void DesbloquearNiveles(){
        for(int i = 0; i < nivel; i++){
            niveles[i].SetActive(true);
       }
    }

    void BloquearNiveles(){
        for(int i = 0; i < nivel; i++){
            niveles[i].SetActive(false);
       }
    }

    public void CambiarPantalla(){
        if(pantalla1.activeSelf){
            pantalla1.SetActive(false);
            pantalla2.SetActive(true);
            DesbloquearNiveles();
        }else{
            for(int i = 0; i < nivel; i++){
                BloquearNiveles();
            }
            pantalla1.SetActive(true);
            pantalla2.SetActive(false);
        }
    }

    public void NuevoJuego(){
        nivel= 1;
        CambiarPantalla();
    }

    public void SeleccionarNivel(int nivel){
        //if(nivel <= this.nivel){
            Debug.Log("Nivel: " + nivel);
            SceneManager.LoadScene(nivel);
        //}else{
        //    Debug.Log("Nivel: no desbloqueado");
        //}
    }

    public void Salir(){
        Application.Quit();

        // Si estás en el editor de Unity, puedes simular el cierre del juego de esta forma:
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
