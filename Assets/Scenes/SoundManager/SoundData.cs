using UnityEngine;

public struct SoundData
{
    public Vector3 position;    // Posición del emisor
    public float volume;        // Volumen base
    public string soundType;    // Tipo de sonido
    public float frequency;     // Frecuencia del sonido (agudo/grave)
    public LayerMask layers;    // Capas que puede atravesar
    public float duration;      // Duración del sonido (0 para instantáneo)
    public object sender;       // Referencia al emisor (opcional)
}
