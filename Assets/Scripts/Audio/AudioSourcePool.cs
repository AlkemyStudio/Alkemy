using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioSourcePool : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSourcePrefab;
        [SerializeField] private int poolSize = 10;
        
        public static AudioSourcePool Instance { get; private set; }
        private List<AudioSource> audioSources;
        
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

        public void PlayClipAtPoint(AudioClip clip, Vector3 position)
        {
            AudioSource audioSource = GetAudioSource();
            audioSource.gameObject.SetActive(true);
            audioSource.transform.position = position;
            audioSource.clip = clip;
            StartCoroutine(Play(audioSource));
        }

        private static IEnumerator Play(AudioSource audioSource)
        {
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
            if (audioSource != null)
            {
                audioSource.gameObject.SetActive(false);
            }
        }

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
