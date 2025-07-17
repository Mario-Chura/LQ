using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LabeledAudioClipData
{
    public string label; // "walk", "attack", etc.
    public AudioClipData[] clips;
}

public class SoundEmitterDragon : MonoBehaviour
{
    [SerializeField] private LayerMask soundLayers = -1;

    [Header("Volume emitter settings")]
    [SerializeField] private float baseVolume = 1f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float frequency = 1f;

    [Header("Audio settings")]
    [SerializeField] public bool is3D = true;
    [SerializeField] public float minDistance = 1f;
    [SerializeField] public float maxDistance = 30f;

    [SerializeField] private LabeledAudioClipData[] labeledClips;

    public void EmitSound(string label)
    {
        // Buscar los clips con la etiqueta correcta
        AudioClipData[] clips = GetClipsByLabel(label);
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"No se encontraron clips para el tipo: {label}");
            return;
        }

        // Elegir uno aleatoriamente
        AudioClipData clipData = clips[Random.Range(0, clips.Length)];

        // Crear y enviar los datos de sonido
        SoundData soundData = new SoundData
        {
            position = transform.position,
            volume = baseVolume,
            soundType = label,
            frequency = frequency,
            layers = soundLayers,
            duration = duration,
            sender = this
        };

        SoundManager.Instance.EmitSound(soundData);
        AudioManager.Instance.PlaySound(clipData, transform.position, baseVolume, minDistance, maxDistance);
    }

    private AudioClipData[] GetClipsByLabel(string label)
    {
        foreach (var entry in labeledClips)
        {
            if (entry.label == label)
                return entry.clips;
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.0f);
    }
}