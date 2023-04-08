using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class ReadyCountdown : MonoBehaviour
    {
        [FormerlySerializedAs("gameManager")] [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private GameObject startGameCanvas;
        [SerializeField] private TextMeshProUGUI textMeshPro;
    
        [SerializeField] private float delayBetweenCountdowns = 1f;
    
        [FormerlySerializedAs("countdownTexts")]
        [Tooltip("The texts to display in the countdown")]
        [SerializeField] private string[] countdownMessages = {
            "3",
            "2",
            "1",
            "FIGHT!"
        };
    
        private int lastDisplayedCountdownMessageIndex = -1;

        private void Awake()
        {
            startGameCanvas.SetActive(false);
        }
    
        private void OnEnable()
        {
            gameStateManager.OnGameStateChanged += OnOnGameStateStateChanged;
        }

        private void OnDisable()
        {
            gameStateManager.OnGameStateChanged -= OnOnGameStateStateChanged;
        }

        private void OnOnGameStateStateChanged(GameState state)
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
            startGameCanvas.SetActive(true);
            lastDisplayedCountdownMessageIndex = -1;
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
            textMeshPro.text = countdownMessage;
        }

        private void StopCountdown()
        {
            CancelInvoke(nameof(OnCountdownTick));
            startGameCanvas.SetActive(false);
            gameStateManager.SetGameState(GameState.Playing);
        }
    }
}
