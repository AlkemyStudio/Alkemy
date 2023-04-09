using System;
using Menu;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Audio
{
    /// <summary>
    /// This script is used to control the volume of an audio mixer parameter.
    /// </summary>
    public class VolumeController : SettingEntry
    {
        [Header("References")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider slider;
        [SerializeField] private Toggle toggle;
        
        [Header("Settings")]
        [SerializeField] private string audioParameterName;
        [SerializeField] [Range(0F, 1F)] private float defaultVolume = 0.5f;
        [SerializeField] private float multiplier = 30;

        private string AudioParameterToggleKey => audioParameterName + "Toggle";
        private float tempSliderValue;
        private bool isUpdatingToggle;
        
        /// <summary>
        /// This method is called when the script instance is being loaded.
        /// </summary>
        private void OnEnable()
        {
            slider.onValueChanged.AddListener(HandleSliderValueChange);
            toggle.onValueChanged.AddListener(HandleToggleValueChanged);
            tempSliderValue = PlayerPrefs.GetFloat(audioParameterName, defaultVolume);
            slider.value = tempSliderValue;
            toggle.isOn = PlayerPrefs.GetInt(AudioParameterToggleKey, 1) == 1;
        }

        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(HandleSliderValueChange);
            toggle.onValueChanged.RemoveListener(HandleToggleValueChanged);
        }
        
        public override void Save()
        {
            PlayerPrefs.SetFloat(audioParameterName, slider.value);
            PlayerPrefs.SetInt(AudioParameterToggleKey, toggle.isOn ? 1 : 0);
        }

        private void HandleSliderValueChange(float value)
        {
            audioMixer.SetFloat(audioParameterName, Mathf.Log10(value) * multiplier);
            
            // if the slider is at the minimum value, mute the audio, otherwise unmute it
            isUpdatingToggle = true;
            toggle.isOn = !(Math.Abs(value - slider.minValue) < 0.001f);
            isUpdatingToggle = false;
        }
        
        private void HandleToggleValueChanged(bool value)
        {
            if (isUpdatingToggle)
                return;

            // if muted, save the current slider value to restore it when unmuting
            if (value)
                tempSliderValue = slider.value;

            slider.value = value ? slider.minValue : tempSliderValue;
        }
    }
}