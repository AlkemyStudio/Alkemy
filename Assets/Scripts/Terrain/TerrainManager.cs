using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Terrain
{
    [RequireComponent(typeof(GameManager))]
    public class TerrainManager : MonoBehaviour
    {
        public static TerrainManager Instance;
        
        public const int Width = 17;
        public const int Height = 11;

        [SerializeField] private GameObject wallPrefab;

        private TerrainEntityType[] _filledTiles;
        private List<GameObject> _previouslyInstantiatedWall;
        
        private GameManager _gameManager;

        private void Awake()
        {
            Instance = this;
            _previouslyInstantiatedWall = new List<GameObject>();
        }

        public void GenerateTerrain()
        {
            ClearOldGeneration();
            GenerateDefaultTerrainData();
            GenerateSpawnsData();
            GenerateWalls();
        }

        private void ClearOldGeneration()
        {
            if (_previouslyInstantiatedWall.Count == 0) return;

            foreach (GameObject wall in _previouslyInstantiatedWall)
            {
                Destroy(wall);
            }
            
            _previouslyInstantiatedWall.Clear();
        }

        private void GenerateDefaultTerrainData()
        {
            int arraySize = Width * Height;
            _filledTiles = new TerrainEntityType[arraySize];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x % 2 == 1 && y % 2 == 1)
                    {
                        SetValue(x, y, TerrainEntityType.IndestructibleEntity);
                    }
                }
            }
        }

        private void GenerateSpawnsData()
        {
            GenerateBottomLeftCornerSpawn();
            GenerateBottomRightCornerSpawn();
            GenerateTopLeftCornerSpawn();
            GenerateTopRightCornerSpawn();
        }

        private void GenerateBottomLeftCornerSpawn()
        {
            SetValue(0, 0, TerrainEntityType.None);
            SetValue(1, 0, TerrainEntityType.None);
            SetValue(0, 1, TerrainEntityType.None);
        }

        private void GenerateBottomRightCornerSpawn()
        {
            SetValue(Width - 1, 0, TerrainEntityType.None);
            SetValue(Width - 2, 0, TerrainEntityType.None);
            SetValue(Width - 1, 1, TerrainEntityType.None);
        }

        private void GenerateTopLeftCornerSpawn()
        {
            SetValue(0, Height - 1, TerrainEntityType.None);
            SetValue(1, Height - 1, TerrainEntityType.None);
            SetValue(0, Height - 2, TerrainEntityType.None);
        }

        private void GenerateTopRightCornerSpawn()
        {
            SetValue(Width - 1, Height - 1, TerrainEntityType.None);
            SetValue(Width - 2, Height - 1, TerrainEntityType.None);
            SetValue(Width - 1, Height - 2, TerrainEntityType.None);
        }

        private void GenerateWalls()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (GetTerrainEntityType(x, y) != TerrainEntityType.Wall) continue;
                    GameObject instantiateGameObject = Instantiate(wallPrefab, new Vector3(x, 0.41f, y), Quaternion.identity);
                    _previouslyInstantiatedWall.Add(instantiateGameObject);
                }
            }
        }

        public Vector2Int GetTilePosition(Vector3 position)
        {
            int tilePosX = Mathf.RoundToInt(position.x);
            int tilePosZ = Mathf.RoundToInt(position.z);
            return new Vector2Int(tilePosX, tilePosZ);
        }

        public void SetValue(int x, int y, TerrainEntityType value)
        {
            _filledTiles[x + y * Width] = value;
        }
        
        public void SetValue(Vector2Int position, TerrainEntityType value)
        {
            _filledTiles[position.x + position.y * Width] = value;
        }

        public bool IsFilled(int x, int y)
        {
            TerrainEntityType terrainEntityType = _filledTiles[x + y * Width];
            return terrainEntityType == TerrainEntityType.None;
        }

        public TerrainEntityType GetTerrainEntityType(int x, int y)
        {
            return _filledTiles[x + y * Width];
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
            if (state != GameState.Initialization) return;
            GenerateTerrain();
            _gameManager.SetGameState(GameState.TerrainGenerated);
        }
        
        private void OnValidate()
        {
            if (_gameManager == null) _gameManager = GetComponent<GameManager>();
        }
    }
}