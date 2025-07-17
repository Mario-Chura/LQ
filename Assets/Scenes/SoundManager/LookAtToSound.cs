using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtToSound : MonoBehaviour
{

    //on start. get the sound listener in the same object
    private SoundListener soundListener;
    private void Start()
    {
        soundListener = GetComponent<SoundListener>();

        soundListener.SetHearingCallback(OnSoundHeard);
    }

    //on sound heard, look at the sound
    private void OnSoundHeard(SoundData soundData, float intensity)
    {
        Vector3 direction = soundData.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);

        // print sound data
        Debug.Log($"Heard sound: {soundData.soundType} at {soundData.position} with intensity {intensity}");
    }


}
