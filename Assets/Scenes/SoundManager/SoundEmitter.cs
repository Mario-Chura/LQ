using UnityEngine;

public class SoundEmitter : MonoBehaviour
{

    [SerializeField] private LayerMask soundLayers = -1;
    [Header("Volume emitter settings")]
    [SerializeField] private float baseVolume = 1f;
    [SerializeField] private float duration = 0f;
    [SerializeField] private float frequency = 1f;

    [Header("Audio settings")]
    [SerializeField] private string soundType = "default";
    [SerializeField] public bool is3D = true;
    [SerializeField] public float minDistance = 1;          // Distancia mínima de audición
    [SerializeField] public float maxDistance = 40;          // Distancia máxima de audición
    [SerializeField] private AudioClipData[] audioClipData;
    public void EmitSound()
    {
        // Crear datos de sonido
        SoundData soundData = new SoundData
        {
            position = transform.position,
            volume = baseVolume,
            soundType = soundType,
            frequency = frequency,
            layers = soundLayers,
            duration = duration,
            sender = this
        };

        // Emitir el sonido al SoundManager
        SoundManager.Instance.EmitSound(soundData);

        // Reproducir el clip de audio correspondiente

        var clipData = GetRandomAudioClipData();

        if (clipData != null)
            AudioManager.Instance.PlaySound(clipData.Value, transform.position, baseVolume, minDistance, maxDistance);
    }

    private AudioClipData? GetRandomAudioClipData()
    {
        if (audioClipData.Length == 0)
            return null;
        return audioClipData[Random.Range(0, audioClipData.Length)];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.0f);
    }
}
