using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSoundDragon : MonoBehaviour
{
    public SoundEmitterDragon footSoundEmitter;

    private void Start()
    {
        footSoundEmitter = GetComponentInParent<SoundEmitterDragon>();
    }

    public void EmitBeforeAfterFlyingSound()
    {
        //Debug.Log("Emitting footstep sound");
        footSoundEmitter.EmitSound("dragon_despegar_aterrizar");
    }
}
