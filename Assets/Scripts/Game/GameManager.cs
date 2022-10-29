
using System;
using UnityEngine;

namespace Game
{

    public class GameManager : MonoBehaviour
    {
        public delegate void GameStateEvent(GameState state);
        public event GameStateEvent GameStateChanged;

        public static GameManager Instance;

        private GameState _currentGameState = GameState.Initialization;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetGameState(GameState.Initialization);
        }

        public void ReloadGame()
        {
            SetGameState(GameState.Initialization);
        }
        
        public void EndTheGame()
        {
            SetGameState(GameState.Ended);
        }

        public void PauseGame()
        {
            SetGameState(GameState.Paused);
        }

        public void ResumeGame()
        {
            SetGameState(GameState.Resumed);
        }

        public void SetGameState(GameState state)
        {
            _currentGameState = state;
            GameStateChanged?.Invoke(state);
        }
    }
}