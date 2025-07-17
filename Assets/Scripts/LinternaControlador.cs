using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinternaControlador : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject linterna;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            //linterna.SetActive(!linterna.activeInHierarchy);

            //no colocamos un condicion si es null, ya que una linterna debe tener estas 2 cosas
            linterna.GetComponent<Light>().enabled = !linterna.GetComponent<Light>().enabled;
            linterna.GetComponent<AudioSource>().Play();
        }

    }
}
