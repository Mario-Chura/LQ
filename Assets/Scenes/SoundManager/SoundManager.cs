using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance => instance;

    public event System.Action<SoundData> OnSoundEmitted;

    private List<ISoundBlocker> soundBlockers = new List<ISoundBlocker>();
    private readonly Queue<RaycastHit[]> raycastHitPool = new Queue<RaycastHit[]>();
    private const int MAX_HITS = 10;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        for (int i = 0; i < 20; i++)
        {
            raycastHitPool.Enqueue(new RaycastHit[MAX_HITS]);
        }
    }

    public void RegisterBlocker(ISoundBlocker blocker)
    {
        if (!soundBlockers.Contains(blocker))
            soundBlockers.Add(blocker);
    }

    public void UnregisterBlocker(ISoundBlocker blocker)
    {
        soundBlockers.Remove(blocker);
    }

    public void EmitSound(SoundData soundData)
    {
        OnSoundEmitted?.Invoke(soundData);
    }

    public float CalculateOcclusion(Vector3 from, Vector3 to, LayerMask layers)
    {
        float occlusion = 1f;
        Vector3 direction = to - from;
        float distance = direction.magnitude;

        RaycastHit[] hits = raycastHitPool.Count > 0
            ? raycastHitPool.Dequeue()
            : new RaycastHit[MAX_HITS];

        int hitCount = Physics.RaycastNonAlloc(from, direction.normalized, hits, distance, layers);

        for (int i = 0; i < hitCount; i++)
        {
            var blocker = hits[i].collider.GetComponent<ISoundBlocker>();
            if (blocker != null)
            {
                occlusion *= blocker.GetOcclusionFactor();
            }
        }

        raycastHitPool.Enqueue(hits);
        return occlusion;
    }
}
