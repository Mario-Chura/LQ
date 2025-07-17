using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundListenerEnemy : SoundListener
{
    public void OnSoundHeard(SoundData soundData, float intensity)
    {
        Debug.Log("Sound heard: " + soundData.soundType + " with intensity " + intensity);
    }
}
