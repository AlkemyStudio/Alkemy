using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class ReadyCountdown : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject textMeshProGameObject;
    
        [SerializeField] private float delayBetweenCountdowns = 1f;
    
        [FormerlySerializedAs("countdownTexts")]
        [Tooltip("The texts to display in the countdown")]
        [SerializeField] private string[] countdownMessages = {
            "3",
            "2",
            "1",
            "FIGHT!"
        };
    
        private TextMeshProUGUI textMeshPro;
        private int lastDisplayedCountdownMessageIndex = -1;

        private void Awake()
        {
            textMeshPro = textMeshProGameObject.GetComponent<TextMeshProUGUI>();
            textMeshProGameObject.SetActive(false);
        }
    
        private void OnEnable()
        {
            gameManager.GameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            gameManager.GameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Ready:
                    StartCountdown();
                    break;
                case GameState.Ended:
                    StopAllCoroutines();
                    break;
                case GameState.Initialization:
                case GameState.TerrainGenerated:
                case GameState.Playing:
                case GameState.Paused:
                case GameState.Resumed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void StartCountdown()
        {
            textMeshProGameObject.SetActive(true);
            InvokeRepeating(nameof(OnCountdownTick), 0f, delayBetweenCountdowns);
        }

        private void OnCountdownTick()
        {
            lastDisplayedCountdownMessageIndex += 1;
            
            if (lastDisplayedCountdownMessageIndex >= countdownMessages.Length) { 
                StopCountdown();
                return;
            }
            
            string countdownMessage = countdownMessages[lastDisplayedCountdownMessageIndex];
            Debug.Log($"Countdown: {countdownMessage}");
            textMeshPro.text = countdownMessage;
        }

        private void StopCountdown()
        {
            CancelInvoke(nameof(OnCountdownTick));
            textMeshProGameObject.SetActive(false);
            gameManager.SetGameState(GameState.Playing);
            enabled = false;
        }
    }
}
