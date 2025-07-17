    using System.Collections.Generic;
    using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance => instance;

        [SerializeField] private int poolSize = 10;

        private Queue<AudioSource> audioSourcePool;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);  // Persistente en la escena

            // Inicializar el pool de AudioSources
            audioSourcePool = new Queue<AudioSource>();
            for (int i = 0; i < poolSize; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 1f;
                audioSourcePool.Enqueue(source);
            }
        }

        public void PlaySound(
            AudioClipData audioClipData, 
            Vector3 position, 
            float volumeMultiplier = 1f, 
            float minDistance = 1f,
            float maxDistance = 50f)
        {
            // Obtener un AudioSource del pool
            AudioSource source = GetAudioSourceFromPool();
            source.clip = audioClipData.clip;
            source.volume = audioClipData.volume * volumeMultiplier;
            source.transform.position = position;

            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.Play();

            // Devolver el AudioSource al pool después de que termine el clip
            StartCoroutine(ReturnToPoolAfterPlayback(source));
        }

        private AudioSource GetAudioSourceFromPool()
        {
            // Si no hay suficientes AudioSources en el pool, crea uno nuevo
            if (audioSourcePool.Count == 0)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                return source;
            }

            return audioSourcePool.Dequeue();
        }

        private System.Collections.IEnumerator ReturnToPoolAfterPlayback(AudioSource source)
        {
            yield return new WaitWhile(() => source.isPlaying);
            audioSourcePool.Enqueue(source);
        }
    }
