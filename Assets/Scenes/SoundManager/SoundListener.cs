using System.Collections.Generic;
using UnityEngine;

public class SoundListener : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Rango de escucha del listener")]
    private float hearingRange = 1f;
    [SerializeField]
    [Tooltip("Distancia máxima a la que el listener puede escuchar")]
    private float maxHearingDistance = 5f;
    [SerializeField]
    [Tooltip("Capas que el listener puede escuchar")]
    private LayerMask hearingLayers = -1;

    private readonly Dictionary<string, float> lastHeardSounds = new Dictionary<string, float>();
    private const float SOUND_MEMORY_TIME = 1f;

    // delegate to be called when a sound is heard
    public delegate void SoundHeard(SoundData soundData, float intensity);
    public event SoundHeard OnSoundHeard;   

    public void SetHearingCallback(SoundHeard callback)
    {
        OnSoundHeard = callback;
    }

    private void OnEnable()
    {
        SoundManager.Instance.OnSoundEmitted += HandleSound;
    }

    private void OnDisable()
    {
        SoundManager.Instance.OnSoundEmitted -= HandleSound;
    }

    private void Update()
    {
        float currentTime = Time.time;
        List<string> soundsToRemove = new List<string>();

        foreach (var kvp in lastHeardSounds)
        {
            if (currentTime - kvp.Value > SOUND_MEMORY_TIME)
                soundsToRemove.Add(kvp.Key);
        }

        foreach (var sound in soundsToRemove)
            lastHeardSounds.Remove(sound);
    }

    private void HandleSound(SoundData soundData)
    {
        //print(soundData.layers.value);
        if ((hearingLayers.value & soundData.layers.value) == 0)
            return;

        // print("Sound detected");
        float distance = Vector3.Distance(transform.position, soundData.position);
        if (distance <= maxHearingDistance * hearingRange)
        {
            //print("Sound in range");
            float intensity = 1 - (distance / (maxHearingDistance * hearingRange));
            intensity *= soundData.volume;

            float occlusion = SoundManager.Instance.CalculateOcclusion(
                soundData.position,
                transform.position,
                soundData.layers
            );

            intensity *= occlusion;

            if (intensity > 0)
            {
                // print("Sound intensity: " + intensity);
                if (!lastHeardSounds.ContainsKey(soundData.soundType) ||
                    Time.time - lastHeardSounds[soundData.soundType] > SOUND_MEMORY_TIME)
                {
                    lastHeardSounds[soundData.soundType] = Time.time;

                    if (OnSoundHeard != null)
                        OnSoundHeard(soundData, intensity);
                }
            }
        }
    }


    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxHearingDistance);
    }
}
