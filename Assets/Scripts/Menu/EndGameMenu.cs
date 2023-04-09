using System.Collections.Generic;
using System.Linq;
using Game;
using PlayerInputs;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Menu
{
    /// <summary>
    /// EndGameMenu is used to display the end game menu.
    /// </summary>
    public class EndGameMenu : MonoBehaviour
    {
        [FormerlySerializedAs("gameManager")]
        [Header("References")]
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private GameObject endGameCanvas;
        [SerializeField] private TextMeshProUGUI winnerText;
        [SerializeField] private AudioSource mainAudioSource;
        [SerializeField] private AudioClip endGameSound;
        private AudioClip _mainAudioClip;

        [Header("Messages Settings")]
        [Tooltip("Use {0} to display the winner name")]
        [SerializeField] private string winnerMessage = "Player {0} Won";
        [SerializeField] private string noWinnerMessage = "No Winner";

        public void Awake()
        {
            _mainAudioClip = mainAudioSource.clip;
            endGameCanvas.SetActive(false);
        }

        private void OnEnable()
        {
            gameStateManager.OnGameEnded += OnGameEnded;
        }
    
        private void OnDisable()
        {
            gameStateManager.OnGameEnded -= OnGameEnded;
        }
    
        /// <summary>
        /// OnGameEnded is called when the game ended.
        /// </summary>
        /// <param name="e"></param>
        private void OnGameEnded(GameEndedEvent e)
        {
            endGameCanvas.SetActive(true);
            mainAudioSource.Stop();
            mainAudioSource.clip = endGameSound;
            mainAudioSource.Play();
            winnerText.text = GetWinnerMessage(e.WinnerNames);
        }
        
        /// <summary>
        /// On restart button clicked.
        /// </summary>
        public void OnRestartButtonClicked()
        {
            endGameCanvas.SetActive(false);
            mainAudioSource.Stop();
            mainAudioSource.clip = _mainAudioClip;
            mainAudioSource.Play();
            gameStateManager.ReloadGame();
        }
        
        /// <summary>
        /// On quit button clicked.
        /// </summary>
        public void OnQuitButtonClicked()
        {
            Destroy(PlayerInputRegistry.Instance.gameObject);
            SceneManager.LoadScene(0);
        }
    
        /// <summary>
        /// Get the winner message.
        /// </summary>
        /// <param name="winnerNames"> The winner names </param>
        /// <returns> The winner message </returns>
        private string GetWinnerMessage(IReadOnlyList<string> winnerNames)
        {
            return winnerNames.Count switch
            {
                1 => string.Format(winnerMessage, winnerNames[0]),
                _ => noWinnerMessage
            };
        }
    }
}
