using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public SoundEmitter footstepSoundEmitter; 

    private void Start()
    {
        footstepSoundEmitter = GetComponentInParent<SoundEmitter>();
    }

    public void EmitFootstepSound()
    {
        //Debug.Log("Emitting footstep sound");
        footstepSoundEmitter.EmitSound();
    }
}
