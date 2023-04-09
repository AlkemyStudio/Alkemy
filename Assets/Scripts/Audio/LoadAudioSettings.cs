using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    /// <summary>
    /// This script is used to load the audio settings from the player prefs.
    /// </summary>
    public class LoadAudioSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private float defaultVolume = 0.2F;
        [SerializeField] private float multiplier = 30;

        /// <summary>
        /// This method is called when the script instance is being loaded.
        /// </summary>
        private void Start()
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * multiplier);
        
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * multiplier);
        
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", defaultVolume);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * multiplier);
        }
    }
}
