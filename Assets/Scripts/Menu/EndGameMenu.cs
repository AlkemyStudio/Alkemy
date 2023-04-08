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
    public class EndGameMenu : MonoBehaviour
    {
        [FormerlySerializedAs("gameManager")]
        [Header("References")]
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private GameObject endGameCanvas;
        [SerializeField] private TextMeshProUGUI winnerText;
    
        [Header("Messages Settings")]
        [Tooltip("Use {0} to display the winner name")]
        [SerializeField] private string winnerMessage = "Player {0} Won";
        [SerializeField] private string noWinnerMessage = "No Winner";

        public void Awake()
        {
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
    
        private void OnGameEnded(GameEndedEvent e)
        {
            endGameCanvas.SetActive(true);
            winnerText.text = GetWinnerMessage(e.WinnerNames);
        }
        
        public void OnRestartButtonClicked()
        {
            endGameCanvas.SetActive(false);
            gameStateManager.ReloadGame();
        }
        
        public void OnQuitButtonClicked()
        {
            Destroy(PlayerInputRegistry.Instance.gameObject);
            SceneManager.LoadScene(0);
        }
    
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
