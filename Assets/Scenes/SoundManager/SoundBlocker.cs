using UnityEngine;

public class SoundBlocker : MonoBehaviour, ISoundBlocker
{
    [SerializeField]
    [Tooltip("Factor de atenuación del sonido. 0 = atenuación total, 1 = sin atenuación")]
    [Range(0, 1)]
    private float occlusionFactor = 0.5f;
    private void OnEnable()
    {
        SoundManager.Instance.RegisterBlocker(this);
    }

    private void OnDisable()
    {
        SoundManager.Instance.UnregisterBlocker(this);
    }

    public float GetOcclusionFactor()
    {
        // clamp occlusion factor to [0, 1]
        return Mathf.Clamp(occlusionFactor, 0, 1);
        
    }
}
