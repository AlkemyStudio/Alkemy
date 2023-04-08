using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Terrain
{
    [RequireComponent(typeof(GameStateManager))]
    public class TerrainManager : MonoBehaviour
    {
        public static TerrainManager Instance;
        
        public const int Width = 17;
        public const int Height = 11;

        [SerializeField] private List<GameObject> wallPrefabs;
        [SerializeField] private GameObject indestructibleWallPrefab;
        [SerializeField] private GameObject floor;

        private TerrainEntityType[] _filledTiles;
        private List<GameObject> _previouslyInstantiated;
        
        private GameStateManager gameStateManager;

        private void Awake()
        {
            Instance = this;
            _previouslyInstantiated = new List<GameObject>();
        }

        public void GenerateTerrain()
        {
            ClearOldGeneration();
            GenerateDefaultTerrainData();
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
            GenerateTerrainBounds();
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

        private void GenerateTerrainBounds()
        {
            GenerateHorizontalBounds();
            GenerateVerticalBounds();
        }

        private void GenerateHorizontalBounds()
        {
            for (int x = -1; x < Width + 1; x++)
            {
                InstantiateIndestructibleWall(x, -1);
                InstantiateIndestructibleWall(x, Height);
            }
        }
        
        private void GenerateVerticalBounds()
        {
            for (int y = 0; y < Height; y++)
            {
                InstantiateIndestructibleWall(-1, y);
                InstantiateIndestructibleWall(Width, y);
            }
        }

        private void GenerateTerrainEntities()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
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

        public bool IsFilled(Vector2Int gridPosition)
        {
            return IsFilled(gridPosition.x, gridPosition.y);
        }
        
        public TerrainEntityType GetTerrainEntityType(int x, int y)
        {
            return _filledTiles[x + y * Width];
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
            if (state != GameState.Initialization) return;
            GenerateTerrain();
            gameStateManager.SetGameState(GameState.TerrainGenerated);
        }
        
        private void OnValidate()
        {
            if (gameStateManager == null) gameStateManager = GetComponent<GameStateManager>();
        }
    }
}