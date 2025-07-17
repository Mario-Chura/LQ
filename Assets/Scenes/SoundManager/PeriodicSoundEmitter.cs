using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class PeriodicSoundEmitter : MonoBehaviour
{
    [SerializeField] private float emissionInterval = 2f; // Tiempo entre emisiones de sonido en segundos
    private float nextEmissionTime = 0f;
    private SoundEmitter soundEmitter;

    private void Start()
    {
        // Obtener el componente SoundEmitter
        soundEmitter = GetComponent<SoundEmitter>();

        nextEmissionTime = Time.time + emissionInterval; // Programar la primera emisión
    }

    private void Update()
    {
        // Emitir sonido si ha pasado el tiempo de intervalo configurado
        if (Time.time >= nextEmissionTime)
        {
            soundEmitter.EmitSound();
            nextEmissionTime = Time.time + emissionInterval; // Programar la siguiente emisión
        }
    }
}
