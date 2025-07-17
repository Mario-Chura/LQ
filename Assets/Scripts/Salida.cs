using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salida : MonoBehaviour
{

    // Text mesh pro text var
    public TMPro.TextMeshProUGUI ganasteText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ganasteText.gameObject.SetActive(true);
            Debug.Log("Ganaste");
            Time.timeScale = 0; 
        }
    }
}
