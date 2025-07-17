using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSoundDragon : MonoBehaviour
{
    public SoundEmitterDragon attackSoundEmitter;

    private void Start()
    {
        attackSoundEmitter = GetComponentInParent<SoundEmitterDragon>();
    }

    public void EmitAttackSound()
    {
        //Debug.Log("Emitting footstep sound");
        attackSoundEmitter.EmitSound("dragon_atacar");
    }
}