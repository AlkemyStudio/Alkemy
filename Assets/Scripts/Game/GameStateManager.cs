
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    public class GameStateManager : MonoBehaviour
    {
        public delegate void GameStateEvent(GameState state);
        public event GameStateEvent OnGameStateChanged;
        public event Action<GameEndedEvent> OnGameEnded;

        public static GameStateManager Instance;

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
        
        public void EndTheGame(string[] winnerNames, string[] lastPlayerNamesDeadAtTheSameTime)
        {
            SetGameState(GameState.Ended);
            OnGameEnded?.Invoke(new GameEndedEvent {
                WinnerNames = winnerNames,
                LastPlayerNamesDeadAtTheSameTime = lastPlayerNamesDeadAtTheSameTime,
                GameEndTime = Time.time
            });
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
            OnGameStateChanged?.Invoke(state);
        }
    }
}