using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Character;
using Lobby;
using Player;
using PlayerInputs;
using Terrain;
using UnityEngine;
using PlayerState = Lobby.PlayerState;

namespace Game
{
    [RequireComponent(typeof(TerrainManager), typeof(GameManager))]
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }
        
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PlayerGameObjectRegistry playerGameObjectRegistry;
        [SerializeField] private GameObject[] playerSpawns;

        private List<GameObject> _instantiatedPlayer;
        private List<PlayerInputHandler> _playerInputHandlers;
        private int _alivePlayerCount;

        private void Awake()
        {
            Instance = this;
            _instantiatedPlayer = new List<GameObject>();
            _playerInputHandlers = new List<PlayerInputHandler>();
        }

        private void GeneratePlayers()
        {
            ClearOldPlayer();
            DoSpawnPlayers();
        }

        private void ClearOldPlayer()
        {
            _playerInputHandlers.Clear();
            foreach (GameObject player in _instantiatedPlayer) {
                Destroy(player);
            }
            _instantiatedPlayer.Clear();
        }
        
        private void DoSpawnPlayers()
        {
            PlayerState[] playerStates = PlayerInputRegistry.Instance.GetPlayerStates();
            
            for (int i = 0; i < playerStates.Length; i++)
            {
                PlayerState playerState = playerStates[i];

                GameObject characterPrefab = playerGameObjectRegistry.GetOneWithIndex(playerState.CharacterIndex);
                int spawnIndex = i % playerSpawns.Length;
                GameObject player = Instantiate(characterPrefab, playerSpawns[spawnIndex].transform.position, Quaternion.identity);
                
                PlayerInputHandler playerInputHandler = playerState.PlayerInput.gameObject.GetComponent<PlayerInputHandler>();
                playerInputHandler.UsePlayerControls();
                player.GetComponent<PlayerMovement>().Initialize(playerInputHandler);
                player.GetComponent<PlayerBombController>().Initialize(playerInputHandler);
                player.GetComponent<EntityHealth>().OnDeath += OnPlayerDeath;
                
                _instantiatedPlayer.Add(player);
                _playerInputHandlers.Add(playerInputHandler);
            }
            
            _alivePlayerCount = _instantiatedPlayer.Count;
        }

        private void OnPlayerDeath(GameObject player)
        {
            Destroy(player);
            _alivePlayerCount--;
            
            if (ShouldEndTheGame())
            {
                gameManager.EndTheGame();
            }
        }
        
        private bool ShouldEndTheGame()
        {
            return _alivePlayerCount <= 1;
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
                case GameState.TerrainGenerated:
                    OnTerrainGenerated();
                    break;
                case GameState.Playing:
                    OnGameStatePlaying();
                    break;
                case GameState.Ended:
                    OnGameEnded();
                    break;
                case GameState.Initialization:
                case GameState.Paused:
                case GameState.Resumed:
                case GameState.Ready:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        private void OnTerrainGenerated()
        {
            GeneratePlayers();
            gameManager.SetGameState(GameState.Ready);
        }

        private void OnGameStatePlaying()
        {
            EnablePlayerInputs();
        }
        
        private void OnGameEnded()
        {
            DisablePlayerInputs();
            StartCoroutine(Restart());
        }

        private IEnumerator Restart()
        {
            yield return new WaitForSeconds(3);
            gameManager.ReloadGame();
        }
        
        private IEnumerator StartPlaying()
        {
            yield return new WaitForSeconds(3);
            gameManager.SetGameState(GameState.Playing);
        }
        
        private void EnablePlayerInputs()
        {
            _playerInputHandlers.ForEach(playerInputHandler => playerInputHandler.EnableInput());
        }
        
        private void DisablePlayerInputs()
        {
            _playerInputHandlers.ForEach(playerInputHandler => playerInputHandler.DisableInput());
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameManager == null) gameManager = GetComponent<GameManager>();
            if (playerGameObjectRegistry == null) playerGameObjectRegistry = GetComponent<PlayerGameObjectRegistry>();
        }  
#endif
    }
}