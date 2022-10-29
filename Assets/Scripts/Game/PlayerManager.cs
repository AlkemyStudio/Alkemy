using System;
using System.Collections;
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
            CheckAlivePlayers();
        }
        
        private void CheckAlivePlayers()
        {
            int alivePlayers = 0;
            
            foreach (GameObject player in _instantiatedPlayer)
            {
                if (player.activeSelf)
                {
                    alivePlayers++;
                }
            }
            
            if (alivePlayers == 0)
            {
                _gameManager.SetGameState(GameState.Ended);
            }
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
            _gameManager.SetGameState(GameState.Ready);
            _gameManager.SetGameState(GameState.Playing);
        }

        private void OnGameEnded()
        {
            StartCoroutine(Restart());
        }

        private IEnumerator Restart()
        {
            yield return new WaitForSeconds(3);
            _gameManager.SetGameState(GameState.Initialization);
        }
        
        private void OnValidate()
        {
            if (_gameManager == null) _gameManager = GetComponent<GameManager>();
        }
    }
}