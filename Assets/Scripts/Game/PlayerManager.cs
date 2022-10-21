using System;
using System.Collections.Generic;
using Character;
using Terrain;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(TerrainManager))]
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        private GameManager _gameManager;
        
        [SerializeField] private GameObject[] playerPrefabs;
        [SerializeField] private GameObject[] playerSpawns;
        private List<GameObject> _instantiatedPlayer;

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
            for (int i = 0; i < playerPrefabs.Length; i++)
            {
                GameObject newPlayer = Instantiate(playerPrefabs[i], playerSpawns[i].transform.position,
                    Quaternion.identity);
                newPlayer.GetComponent<EntityHealth>().OnDeath += OnPlayerDeath;
                _instantiatedPlayer.Add(newPlayer);
            }
        }

        private void OnPlayerDeath(GameObject player)
        {
            Destroy(player);
        }
        
        private void OnEnable()
        {
            _gameManager.GameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            _gameManager.GameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state != GameState.TerrainGenerated) return;
            GeneratePlayers();
            _gameManager.SetGameState(GameState.Ready);
            _gameManager.SetGameState(GameState.Playing);
        }
        
        private void OnValidate()
        {
            if (_gameManager == null) _gameManager = GetComponent<GameManager>();
        }
    }
}