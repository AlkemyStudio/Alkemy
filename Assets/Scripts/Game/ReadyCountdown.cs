using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    /// <summary>
    /// A countdown that is displayed before the game starts.
    /// </summary>
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

        /// <summary>
        /// Disable the countdown canvas on awake.
        /// </summary>
        private void Awake()
        {
            startGameCanvas.SetActive(false);
        }
    
        /// <summary>
        /// Subscribe to the game state changed event.
        /// </summary>
        private void OnEnable()
        {
            gameStateManager.OnGameStateChanged += OnOnGameStateStateChanged;
        }

        /// <summary>
        /// Unsubscribe from the game state changed event.
        /// </summary>
        private void OnDisable()
        {
            gameStateManager.OnGameStateChanged -= OnOnGameStateStateChanged;
        }

        /// <summary>
        /// Start the countdown when the game state is ready.
        /// </summary>
        /// <param name="state"> The new game state. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when the game state is not handled. </exception>
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

        /// <summary>
        /// Start the countdown.
        /// </summary>
        private void StartCountdown()
        {
            startGameCanvas.SetActive(true);
            lastDisplayedCountdownMessageIndex = -1;
            InvokeRepeating(nameof(OnCountdownTick), 0f, delayBetweenCountdowns);
        }

        /// <summary>
        /// Display the next countdown message.
        /// </summary>
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

        /// <summary>
        /// Stop the countdown.
        /// </summary>
        private void StopCountdown()
        {
            CancelInvoke(nameof(OnCountdownTick));
            startGameCanvas.SetActive(false);
            gameStateManager.SetGameState(GameState.Playing);
        }
    }
}
