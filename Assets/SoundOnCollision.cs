using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    [SerializeField]
    private SoundEmitter soundEmitter;
    [SerializeField]
    public float minVelocitySound = 1f;


    // initialization
    private void Start()
    {
        // Get the SoundEmitter component
    }

    // collision detection. if velocity is greater than 0.5, emit sound
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > minVelocitySound)
        {
            soundEmitter.EmitSound();
        }
    }
}
