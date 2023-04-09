using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// A pool of AudioSources that can be used to play audio clips at a specific position.
    /// </summary>
    public class AudioSourcePool : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSourcePrefab;
        [SerializeField] private int poolSize = 10;
        
        public static AudioSourcePool Instance { get; private set; }
        private List<AudioSource> audioSources;
        
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
#if UNITY_EDITOR
            if (Instance != null)
            {
                Debug.LogError("Multiple instances of AudioSourcePool");
            }
#endif
            Instance = this;
            
            audioSources = new List<AudioSource>();
            for (int i = 0; i < poolSize; i++)
            {
                AudioSource audioSource = Instantiate(audioSourcePrefab, transform);
                audioSource.gameObject.SetActive(false);
                audioSources.Add(audioSource);
            }
        }

        /// <summary>
        /// Plays the given clip at the given position.
        /// </summary>
        /// <param name="clip">The clip to play.</param>
        /// <param name="position">The position to play the clip at.</param>
        public void PlayClipAtPoint(AudioClip clip, Vector3 position)
        {
            AudioSource audioSource = GetAudioSource();
            audioSource.gameObject.SetActive(true);
            audioSource.transform.position = position;
            audioSource.clip = clip;
            StartCoroutine(Play(audioSource));
        }

        /// <summary>
        /// Plays the given clip at the given position.
        /// </summary>
        /// <param name="audioSource">The audio source to play.</param>
        /// <returns></returns>
        private static IEnumerator Play(AudioSource audioSource)
        {
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
            if (audioSource != null)
            {
                audioSource.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Gets an available audio source from the pool.
        /// </summary>
        /// <returns>An available audio source.</returns>
        private AudioSource GetAudioSource()
        {
            foreach (AudioSource audioSource in audioSources)
            {
                if (!audioSource.gameObject.activeInHierarchy)
                {
                    return audioSource;
                }
            }

            AudioSource newAudioSource = Instantiate(audioSourcePrefab, transform);
            audioSources.Add(newAudioSource);
            return newAudioSource;
        }
    }
}
