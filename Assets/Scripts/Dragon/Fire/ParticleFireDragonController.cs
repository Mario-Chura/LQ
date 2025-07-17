using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFireDragonController : MonoBehaviour
{
    private ParticleSystem particlesFire;
    private AudioSource audioFuego;
    //private BoxCollider boxColliderFire;
    private bool gameover; 

    private void Start()
    {
        particlesFire = GetComponent<ParticleSystem>();
        audioFuego = GetComponent<AudioSource>();
        //boxColliderFire = GetComponent<BoxCollider>();

        gameover = false;
    }

    public void activarParticulasFuego()
    {
        particlesFire.Play();
        audioFuego.Play();
        //boxColliderFire.enabled = true;
    }

    public void desactivarParticulasFuego()
    {
        particlesFire.Stop();
        audioFuego.Stop();
        //boxColliderFire.enabled = false;
    }


    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player") && !gameover)
        {
            Debug.Log("Game Over");
            gameover = true;
            PlayerSingleton.Instance.GameOver(); //se acaba el juego
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over...");

            PlayerSingleton.Instance.GameOver(); //se acaba el juego
        }
    }*/
}
