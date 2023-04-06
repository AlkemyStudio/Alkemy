using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using Lobby;
using Player;
using Terrain;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(TerrainManager), typeof(GameManager))]
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        [SerializeField] private GameManager gameManager;

        private List<GameObject> _instantiatedPlayer;
        private int _alivePlayerCount;

        private void Awake()
        {
            Instance = this;
            _instantiatedPlayer = new List<GameObject>();
        }

        private void GeneratePlayers()
        {
            ClearOldPlayer();
            DoSpawnPlayers();
        }

        private void ClearOldPlayer()
        {
            foreach (GameObject player in _instantiatedPlayer)
            {
                Destroy(player);
            }
            
            _instantiatedPlayer.Clear();
        }
        
        private void DoSpawnPlayers()
        {
#if UNITY_EDITOR
            if (PlayerConfigurationManager.Instance == null)
            {
                SpawnDebugPlayers();
            } else {
                SpawnPlayers();
            }
#else
            SpawnPlayers();
#endif  
        }

        private void SpawnPlayers()
        {
            SelectableCharacterRegistry selectableCharacterRegistry = SelectableCharacterRegistry.Instance;
            List<Vector2Int> playerSpawnGridPositions = TerrainManager.Instance.PlayerSpawnGridPositions;
            List<PlayerConfiguration> playerConfigurations = PlayerConfigurationManager.Instance.PlayerConfigurations;

            for (
                int playerConfigurationIndex = 0; 
                playerConfigurationIndex < playerConfigurations.Count; 
                playerConfigurationIndex++
            ) {
                PlayerConfiguration playerConfiguration = playerConfigurations[playerConfigurationIndex];
                
                GameObject characterPrefab = GetCharacterPrefab(
                    selectableCharacterRegistry,
                    playerConfiguration.CharacterIndex
                );
                
                Vector2Int spawnGridPosition = GetSpawnGridPositionFrom(
                    playerConfigurationIndex,
                    playerSpawnGridPositions
                );
                
                SpawnPlayer(
                    playerConfiguration,
                    characterPrefab,
                    spawnGridPosition
                );
            }
            
            _alivePlayerCount = _instantiatedPlayer.Count;
        }
        
#if UNITY_EDITOR
        private void SpawnDebugPlayers()
        {
            SelectableCharacterRegistry selectableCharacterRegistry = SelectableCharacterRegistry.Instance;
            List<Vector2Int> playerSpawnGridPositions = TerrainManager.Instance.PlayerSpawnGridPositions;
            Debug.Log("You should run the Lobby scene first in order to spawn players.\nDebug game will spawn 4 players.");
            SpawnPlayer(
                new PlayerConfiguration(null, 0),
                GetCharacterPrefab(selectableCharacterRegistry, 0),
                GetSpawnGridPositionFrom(0, playerSpawnGridPositions)
            );
            SpawnPlayer(
                new PlayerConfiguration(null, 1),
                GetCharacterPrefab(selectableCharacterRegistry, 1),
                GetSpawnGridPositionFrom(1, playerSpawnGridPositions)
            );
            SpawnPlayer(
                new PlayerConfiguration(null, 2),
                GetCharacterPrefab(selectableCharacterRegistry, 2),
                GetSpawnGridPositionFrom(2, playerSpawnGridPositions)
            );
            SpawnPlayer(
                new PlayerConfiguration(null, 3),
                GetCharacterPrefab(selectableCharacterRegistry, 3),
                GetSpawnGridPositionFrom(3, playerSpawnGridPositions)
            );
        }
#endif

        private void SpawnPlayer(
            PlayerConfiguration playerConfiguration,
            GameObject characterPrefab,
            Vector2Int spawnGridPosition
        ) {
            GameObject player = InstantiatePlayerAt(spawnGridPosition, characterPrefab);
            ConfigurePlayer(player, playerConfiguration);
            _instantiatedPlayer.Add(player);
        }

        private GameObject GetCharacterPrefab(SelectableCharacterRegistry registry, int characterIndex)
        {
            return registry.GetSelectableCharacterAt(characterIndex).prefab;
        }

        private void ConfigurePlayer(GameObject player, PlayerConfiguration playerConfiguration)
        {
            GameObject playerConfigurationGameObject = playerConfiguration.Input.gameObject;
            player.GetComponent<EntityHealth>().OnDeath += OnPlayerDeath;
            playerConfigurationGameObject.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfiguration, player);
        }

        private static Vector2Int GetSpawnGridPositionFrom(int playerConfigurationIndex, IReadOnlyList<Vector2Int> playerSpawnGridPositions)
        {
            int spawnIndex = playerConfigurationIndex % playerSpawnGridPositions.Count;
            return playerSpawnGridPositions[spawnIndex];
        }
        
        private GameObject InstantiatePlayerAt(Vector2Int spawnGridPosition, GameObject characterPrefab)
        {
            return Instantiate(
                characterPrefab, 
                GetSpawnPositionFrom(spawnGridPosition),
                Quaternion.identity
            );
        }
        
        private Vector3 GetSpawnPositionFrom(Vector2Int spawnGridPosition)
        {
            return new Vector3(spawnGridPosition.x + 0.5F, 0.6F, spawnGridPosition.y + 0.5F);
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
                case GameState.Ended:
                    OnGameEnded();
                    break;
                case GameState.Initialization:
                case GameState.Ready:
                case GameState.Playing:
                case GameState.Paused:
                case GameState.Resumed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        private void OnTerrainGenerated()
        {
            GeneratePlayers();
            gameManager.SetGameState(GameState.Ready);
            gameManager.SetGameState(GameState.Playing);
        }

        private void OnGameEnded()
        {
            StartCoroutine(Restart());
        }

        private IEnumerator Restart()
        {
            yield return new WaitForSeconds(3);
            gameManager.ReloadGame();
        }
        
        private void OnValidate()
        {
            if (gameManager == null) gameManager = GetComponent<GameManager>();
        }
    }
}