using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Character;
using Lobby;
using Player;
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
            List<PlayerState> playerStates = LobbyController.Instance.LobbyPlayerStates
                .Select(state => state.Value)
                .ToList();
            
            for (int i = 0; i < playerStates.Count; i++)
            {
                PlayerState playerState = playerStates[i];

                GameObject characterPrefab = playerGameObjectRegistry.GetOneWithIndex(playerState.CharacterIndex);
                int spawnIndex = i % playerSpawns.Length;
                GameObject player = Instantiate(characterPrefab, playerSpawns[spawnIndex].transform.position, Quaternion.identity);
                
                PlayerInputHandler playerInputHandler = playerState.PlayerInput.gameObject.GetComponent<PlayerInputHandler>();
                playerInputHandler.EnableInput();
                playerInputHandler.UsePlayerControls();
                player.GetComponent<PlayerMovement>().Initialize(playerInputHandler);
                player.GetComponent<PlayerBombController>().Initialize(playerInputHandler);
                player.GetComponent<EntityHealth>().OnDeath += OnPlayerDeath;
                
                _instantiatedPlayer.Add(player);
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
            if (_alivePlayerCount > 1) return false;
            return true;
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameManager == null) gameManager = GetComponent<GameManager>();
            if (playerGameObjectRegistry == null) playerGameObjectRegistry = GetComponent<PlayerGameObjectRegistry>();
        }  
#endif
    }
}