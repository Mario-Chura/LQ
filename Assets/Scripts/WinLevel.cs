using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinLevel : MonoBehaviour
{   void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSingleton.Instance.WinGame();
            Debug.Log("You win!");
        }
    }
}
