using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayMusicQueue : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private bool loopLastSong;
        [SerializeField] private float delayBetweenSongs;
        [SerializeField] private List<AudioClip> musicQueue;

        private void Start()
        {
            PlaySong();
        }

        private void PlaySong()
        {
            if (musicQueue.Count == 0) return;
        
            audioSource.clip = musicQueue[0];
            audioSource.Play();

            if (IsLastSongAndShouldLoopLastSong())
            {
                audioSource.loop = true;
                return;
            }
            
            StartCoroutine(PlayNextSong());
        }
        
        private bool IsLastSongAndShouldLoopLastSong()
        {
            return loopLastSong && musicQueue.Count == 1;
        }

        private IEnumerator PlayNextSong()
        {
            yield return new WaitForSeconds(audioSource.clip.length + delayBetweenSongs);
            if (!IsLastSongAndShouldLoopLastSong()) {
                musicQueue.RemoveAt(0);
            }
            PlaySong();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }
#endif
    }
}
