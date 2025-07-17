using System.Collections;
using UnityEngine;

public class Random3DAudioPlayer : MonoBehaviour
{
    // Lista de clips de audio asignados desde el inspector
    public AudioClipData[] audioClipDatas;

    // Componente AudioSource para reproducir los clips
    private AudioSource audioSource;

    // Espaciado aleatorio entre reproducciones (entre 0 y 3 segundos)
    [Range(0f, 10f)]
    public float minDelay = 0f;
    [Range(0f, 10f)]
    public float maxDelay = 3f;

    // Configuración del alcance del sonido 3D
    public float minDistance = 5f;  // Distancia mínima para escuchar el audio claramente
    public float maxDistance = 50f; // Distancia máxima en la que se escucha el audio (atenuado)
    public bool is3D = true;        // ¿El audio es 3D o 2D?

    void Start()
    {
        // Obtiene el AudioSource adjunto al GameObject o lo añade si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configura el AudioSource para sonido en 3D
        audioSource.spatialBlend = is3D ? 1f : 0f; // 1f para 3D, 0f para 2D
        audioSource.playOnAwake = false;    // No reproducir automáticamente al iniciar
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.rolloffMode = AudioRolloffMode.Linear; // Atenuación lineal para mayor control

        // Inicia la reproducción de audio aleatorio
        StartCoroutine(PlayRandomAudio());
    }

    IEnumerator PlayRandomAudio()
    {
        while (true)
        {
            if (audioClipDatas.Length > 0)
            {
                // Selecciona un clip aleatorio de la lista
                AudioClipData audioClipData = audioClipDatas[Random.Range(0, audioClipDatas.Length)];
                AudioClip randomClip = audioClipData.clip;

                // Reproduce el clip seleccionado en la posición del objeto
                // volume
                audioSource.volume = audioClipData.volume;
                audioSource.PlayOneShot(randomClip);

                // Espera hasta que termine el clip más un intervalo aleatorio entre minDelay y maxDelay
                float waitTime = randomClip.length + Random.Range(minDelay, maxDelay);
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                Debug.LogWarning("No hay clips de audio asignados.");
                yield break;
            }
        }
    }
}
