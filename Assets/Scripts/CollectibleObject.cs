using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleObject : MonoBehaviour
{
    private bool isPlayerNearby = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que tu jugador tenga el tag "Player".
        {
            isPlayerNearby = true;
            ShowPickupMessage(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            ShowPickupMessage(false);
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    private void ShowPickupMessage(bool show)
    {
        // Implementa aquí cómo mostrar/ocultar el mensaje en tu UI
        Debug.Log(show ? "Presiona 'E' para recoger el objeto" : "");
    }

    private void Pickup()
    {
        // Aquí puedes notificar al jugador que tiene el objeto
        Debug.Log("Objeto recogido");
        gameObject.SetActive(false); // Desactiva el objeto en la escena
    }
}
