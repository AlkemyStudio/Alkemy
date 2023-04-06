using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Terrain
{
    [RequireComponent(typeof(GameManager))]
    public class TerrainManager : MonoBehaviour
    {
        public static TerrainManager Instance;
        
        public List<Vector2Int> PlayerSpawnGridPositions { get; private set; }
        public int width = 19;
        public int height = 13;

        [Header("Terrain Settings")]
        [SerializeField] private GameObject floor;
        [SerializeField] private GameObject indestructibleWallPrefab;
        [SerializeField] private List<GameObject> wallPrefabs;


        private TerrainEntityType[] _filledTiles;
        private List<GameObject> _previouslyInstantiated;

        private GameManager _gameManager;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one TerrainManager in scene!");
            }
            Instance = this;

            PlayerSpawnGridPositions = new List<Vector2Int>();
            _previouslyInstantiated = new List<GameObject>();
        }

        public void GenerateTerrain()
        {
            ClearOldGeneration();
            GenerateDefaultTerrainData();
            GenerateTerrainBorder();
            GenerateSpawnsData();
            GenerateTerrainEntities();
        }

        private void ClearOldGeneration()
        {
            if (_previouslyInstantiated.Count == 0) return;

            foreach (GameObject wall in _previouslyInstantiated)
            {
                Destroy(wall);
            }
            
            _previouslyInstantiated.Clear();
            PlayerSpawnGridPositions.Clear();
        }

        private void GenerateDefaultTerrainData()
        {
            int arraySize = width * height;
            _filledTiles = new TerrainEntityType[arraySize];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        SetValue(x, y, TerrainEntityType.IndestructibleEntity);
                    }
                }
            }
        }

        private void GenerateTerrainBorder()
        {
            for (int x = 0; x < width; x++)
            {
                SetValue(x, 0, TerrainEntityType.IndestructibleEntity);
                SetValue(x, height - 1, TerrainEntityType.IndestructibleEntity);
            }
            
            for (int y = 0; y < height; y++)
            {
                SetValue(0, y, TerrainEntityType.IndestructibleEntity);
                SetValue(width - 1, y, TerrainEntityType.IndestructibleEntity);
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
            PlayerSpawnGridPositions.Add(new Vector2Int(1, 1));
            SetValue(1, 1, TerrainEntityType.None);
            SetValue(2, 1, TerrainEntityType.None);
            SetValue(1, 2, TerrainEntityType.None);
        }

        private void GenerateBottomRightCornerSpawn()
        {
            PlayerSpawnGridPositions.Add(new Vector2Int(width - 2, 1));
            SetValue(width - 2, 1, TerrainEntityType.None);
            SetValue(width - 3, 1, TerrainEntityType.None);
            SetValue(width - 2, 2, TerrainEntityType.None);
        }

        private void GenerateTopLeftCornerSpawn()
        {
            PlayerSpawnGridPositions.Add(new Vector2Int(1, height - 2));
            SetValue(1, height - 2, TerrainEntityType.None);
            SetValue(2, height - 2, TerrainEntityType.None);
            SetValue(1, height - 3, TerrainEntityType.None);
        }

        private void GenerateTopRightCornerSpawn()
        {
            PlayerSpawnGridPositions.Add(new Vector2Int(width - 2, height - 2));
            SetValue(width - 2, height - 2, TerrainEntityType.None);
            SetValue(width - 3, height - 2, TerrainEntityType.None);
            SetValue(width - 2, height - 3, TerrainEntityType.None);
        }

        private void GenerateTerrainEntities()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    TerrainEntityType entityType = GetTerrainEntityType(x, y);

                    switch (entityType)
                    {
                        case TerrainEntityType.Wall:
                            InstantiateRandomWall(x, y);
                            break;
                        case TerrainEntityType.None:
                            InstantiateFloor(x, y);
                            break;
                        case TerrainEntityType.IndestructibleEntity:
                            InstantiateIndestructibleWall(x, y);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public void InstantiateFloor(int x, int y)
        {
            InstantiateEntity(floor, x, y);
        }

        public void InstantiateRandomWall(int x, int y)
        {
            InstantiateEntity(wallPrefabs[Random.Range(0, wallPrefabs.Count)], x, y);
        }

        public void InstantiateIndestructibleWall(int x, int y)
        {
            InstantiateEntity(indestructibleWallPrefab, x, y);
        }

        public void InstantiateEntity(GameObject entity, int x, int y)
        {
            GameObject instantiateGameObject = Instantiate(entity, new Vector3(x + 0.5F, 0, y + 0.5F), Quaternion.identity);
            _previouslyInstantiated.Add(instantiateGameObject);
        }

        public void SetValue(int x, int y, TerrainEntityType value)
        {
            _filledTiles[x + y * width] = value;
        }
        
        public void SetValue(Vector2Int position, TerrainEntityType value)
        {
            _filledTiles[position.x + position.y * width] = value;
        }

        public bool IsFilled(int x, int y)
        {
            TerrainEntityType terrainEntityType = _filledTiles[x + y * width];
            return terrainEntityType == TerrainEntityType.None;
        }

        public bool IsFilled(Vector2Int gridPosition)
        {
            return IsFilled(gridPosition.x, gridPosition.y);
        }
        
        public TerrainEntityType GetTerrainEntityType(int x, int y)
        {
            return _filledTiles[x + y * width];
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