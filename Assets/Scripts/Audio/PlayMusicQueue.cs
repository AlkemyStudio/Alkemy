using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// This script is used to play a queue of songs.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PlayMusicQueue : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private bool loopLastSong;
        [SerializeField] private float delayBetweenSongs;
        [SerializeField] private List<AudioClip> musicQueue;

        /// <summary>
        /// This method is called when the script instance is being loaded.
        /// </summary>
        private void Start()
        {
            PlaySong();
        }

        /// <summary>
        /// This method is used to play the next song in the queue.
        /// </summary>
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
        
        /// <summary>
        /// This method is used to check if the last song should be looped.
        /// </summary>
        /// <returns></returns>
        private bool IsLastSongAndShouldLoopLastSong()
        {
            return loopLastSong && musicQueue.Count == 1;
        }

        /// <summary>
        /// This method is used to play the next song in the queue.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayNextSong()
        {
            yield return new WaitForSeconds(audioSource.clip.length + delayBetweenSongs);
            if (!IsLastSongAndShouldLoopLastSong()) {
                musicQueue.RemoveAt(0);
            }
            PlaySong();
        }

#if UNITY_EDITOR
        /// <summary>
        /// This method is called when the script is loaded or a value is changed in the inspector.
        /// </summary>
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
