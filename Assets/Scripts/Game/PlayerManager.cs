using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Lobby;
using Player;
using PlayerInputs;
using Terrain;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using PlayerState = Lobby.PlayerState;

namespace Game
{
    [RequireComponent(typeof(TerrainManager), typeof(GameStateManager))]
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }
        
        [Header("References")]
        [FormerlySerializedAs("gameManager")] [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private PlayerGameObjectRegistry playerGameObjectRegistry;
        [SerializeField] private GameObject[] playerSpawns;
        [SerializeField] private GameObject explosionPrefab;

        [Header("Settings")]
        [SerializeField] private float timeBeforeEndGame = 2f;
        [SerializeField] private float equalityThreshold = 0.1f;
        
        private List<GameObject> _instantiatedPlayer;
        private List<PlayerInputHandler> _playerInputHandlers;
        private int _alivePlayerCount;
        
        private float _lastDeathTime;
        private Dictionary<string, float> _playerDeathTimes;

        private void Awake()
        {
            Instance = this;
            _instantiatedPlayer = new List<GameObject>();
            _playerInputHandlers = new List<PlayerInputHandler>();
            _playerDeathTimes = new Dictionary<string, float>();
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
#if UNITY_EDITOR
            // In the editor, we don't have a PlayerInputRegistry, so to avoid errors, we just return.
            if (PlayerInputRegistry.Instance == null)
                return;
#endif
            PlayerState[] playerStates = PlayerInputRegistry.Instance.GetPlayerStates();
            
            for (int i = 0; i < playerStates.Length; i++)
            {
                PlayerState playerState = playerStates[i];

                GameObject characterPrefab = playerGameObjectRegistry.GetOneWithIndex(playerState.CharacterIndex);
                int spawnIndex = i % playerSpawns.Length;
                GameObject player = Instantiate(characterPrefab, playerSpawns[spawnIndex].transform.position, Quaternion.identity);
                player.name = $"Player {playerState.PlayerInput.playerIndex.ToString()}";
                
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
            VoxelParser voxelParser = player.GetComponentInChildren<VoxelParser>();

            if (voxelParser == null)
            {
                Debug.LogError("Player has no VoxelParser");
                _lastDeathTime = Time.time;
                _playerDeathTimes[player.name] = _lastDeathTime;
                
                Destroy(player);
                _alivePlayerCount--;
            
                if (ShouldEndTheGame())
                {
                    StartCoroutine(EndGame());
                }

                return;
            }

            GameObject explosion = Instantiate(explosionPrefab, player.transform.position, Quaternion.identity);

            VoxelGraph voxelGraph = explosion.GetComponent<VoxelGraph>();
            voxelGraph.voxelParser = voxelParser;
            
            _lastDeathTime = Time.time;
            _playerDeathTimes[player.name] = _lastDeathTime;
            
            Destroy(player);

            voxelGraph.OnAnimationEnd += () => {
                _alivePlayerCount--;
            
                if (ShouldEndTheGame())
                {
                    StartCoroutine(EndGame());
                }
            };

            voxelGraph.enabled = true;
        }
        
        private IEnumerator EndGame()
        {
            yield return new WaitForSeconds(timeBeforeEndGame);
            
            GameObject alivePlayer = GameObject.FindWithTag("Player");
            string[] winnerNames = alivePlayer == null 
                ? Array.Empty<string>() 
                : new[] { alivePlayer.name };
            
            gameStateManager.EndTheGame(winnerNames, GetLastPlayerNamesDeadAtTheSameTime());
        }
        
        private string[] GetLastPlayerNamesDeadAtTheSameTime()
        {
            return _playerDeathTimes
                .Where(pair => _lastDeathTime - pair.Value < equalityThreshold)
                .Select(pair => pair.Key)
                .ToArray();
        }

        private bool ShouldEndTheGame()
        {
            return _alivePlayerCount <= 1;
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
            gameStateManager.SetGameState(GameState.Ready);
        }

        private void OnGameStatePlaying()
        {
            EnablePlayerInputs();
        }
        
        private void OnGameEnded()
        {
            DisablePlayerInputs();
        }
        
        private IEnumerator StartPlaying()
        {
            yield return new WaitForSeconds(3);
            gameStateManager.SetGameState(GameState.Playing);
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
            if (gameStateManager == null) gameStateManager = GetComponent<GameStateManager>();
            if (playerGameObjectRegistry == null) playerGameObjectRegistry = GetComponent<PlayerGameObjectRegistry>();
        }  
#endif
    }
}