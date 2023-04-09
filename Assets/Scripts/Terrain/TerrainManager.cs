using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Terrain
{
    /// <summary>
    /// This class is used to manage the terrain
    /// </summary>
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
        
        [SerializeField] private GameStateManager gameStateManager;

        private void Awake()
        {
            Instance = this;
            _previouslyInstantiated = new List<GameObject>();
        }

        /// <summary>
        /// This method is used to generate the terrain
        /// </summary>
        public void GenerateTerrain()
        {
            ClearOldGeneration();
            GenerateDefaultTerrainData();
            GenerateSpawnsData();
            GenerateTerrainEntities();
        }

        /// <summary>
        /// This method is used to instantiate a floor
        /// </summary>
        private void ClearOldGeneration()
        {
            if (_previouslyInstantiated.Count == 0) return;

            foreach (GameObject wall in _previouslyInstantiated)
            {
                Destroy(wall);
            }
            
            _previouslyInstantiated.Clear();
        }

        /// <summary>
        /// This method is used to instantiate a floor
        /// </summary>
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

        /// <summary>
        /// This method is used to instantiate a floor
        /// </summary>
        private void GenerateSpawnsData()
        {
            GenerateBottomLeftCornerSpawn();
            GenerateBottomRightCornerSpawn();
            GenerateTopLeftCornerSpawn();
            GenerateTopRightCornerSpawn();
            GenerateTerrainBounds();
        }

        /// <summary>
        /// This method is used to instantiate a floor
        /// </summary>
        private void GenerateBottomLeftCornerSpawn()
        {
            SetValue(0, 0, TerrainEntityType.None);
            SetValue(1, 0, TerrainEntityType.None);
            SetValue(0, 1, TerrainEntityType.None);
        }

        /// <summary>
        /// This method is used to instantiate a floor
        /// </summary>
        private void GenerateBottomRightCornerSpawn()
        {
            SetValue(Width - 1, 0, TerrainEntityType.None);
            SetValue(Width - 2, 0, TerrainEntityType.None);
            SetValue(Width - 1, 1, TerrainEntityType.None);
        }

        /// <summary>
        /// This method is used to instantiate a floor
        /// </summary>
        private void GenerateTopLeftCornerSpawn()
        {
            SetValue(0, Height - 1, TerrainEntityType.None);
            SetValue(1, Height - 1, TerrainEntityType.None);
            SetValue(0, Height - 2, TerrainEntityType.None);
        }

        /// <summary>
        /// This method is used to instantiate a floor
        /// </summary>
        private void GenerateTopRightCornerSpawn()
        {
            SetValue(Width - 1, Height - 1, TerrainEntityType.None);
            SetValue(Width - 2, Height - 1, TerrainEntityType.None);
            SetValue(Width - 1, Height - 2, TerrainEntityType.None);
        }

        /// <summary>
        /// Generate the bounds of the terrain
        /// </summary>
        private void GenerateTerrainBounds()
        {
            GenerateHorizontalBounds();
            GenerateVerticalBounds();
        }

        /// <summary>
        /// Generate the horizontal bounds of the terrain
        /// </summary>
        private void GenerateHorizontalBounds()
        {
            for (int x = -1; x < Width + 1; x++)
            {
                InstantiateIndestructibleWall(x, -1);
                InstantiateIndestructibleWall(x, Height);
            }
        }

        /// <summary>
        /// Generate the vertical bounds of the terrain
        /// </summary>
        private void GenerateVerticalBounds()
        {
            for (int y = 0; y < Height; y++)
            {
                InstantiateIndestructibleWall(-1, y);
                InstantiateIndestructibleWall(Width, y);
            }
        }

        /// <summary>
        /// This method is used to generate the terrain entities
        /// </summary>
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

        /// <summary>
        /// This method is used to instantiate a floor at the given position
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public void InstantiateFloor(int x, int y)
        {
            InstantiateEntity(floor, x, y);
        }

        /// <summary>
        /// This method is used to instantiate a random wall at the given position
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public void InstantiateRandomWall(int x, int y)
        {
            InstantiateEntity(wallPrefabs[Random.Range(0, wallPrefabs.Count)], x, y);
        }

        /// <summary>
        /// This method is used to instantiate an indestructible wall at the given position
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public void InstantiateIndestructibleWall(int x, int y)
        {
            InstantiateEntity(indestructibleWallPrefab, x, y);
        }

        /// <summary>
        /// This method is used to instantiate an entity at the given position
        /// </summary>
        /// <param name="entity"> entity to instantiate</param>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public void InstantiateEntity(GameObject entity, int x, int y)
        {
            GameObject instantiateGameObject = Instantiate(entity, new Vector3(x + 0.5F, 0, y + 0.5F), Quaternion.identity);
            _previouslyInstantiated.Add(instantiateGameObject);
        }

        /// <summary>
        /// Set the value of the given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void SetValue(int x, int y, TerrainEntityType value)
        {
            _filledTiles[x + y * Width] = value;
        }
        
        /// <summary>
        /// Set the value of the given position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        public void SetValue(Vector2Int position, TerrainEntityType value)
        {
            _filledTiles[position.x + position.y * Width] = value;
        }

        /// <summary>
        /// Is the given position filled
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsFilled(int x, int y)
        {
            TerrainEntityType terrainEntityType = _filledTiles[x + y * Width];
            return terrainEntityType == TerrainEntityType.None;
        }

        
        /// <summary>
        /// Is the given position filled
        /// </summary>
        /// <param name="gridPosition">the position to check</param>
        /// <returns>if the given position is filled</returns>
        public bool IsFilled(Vector2Int gridPosition)
        {
            return IsFilled(gridPosition.x, gridPosition.y);
        }
        
        /// <summary>
        /// Get the terrain entity type at the given position
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <returns></returns>
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

        /// <summary>
        /// On game state changed
        /// </summary>
        /// <param name="state">the new state of the game</param>
        private void OnOnGameStateStateChanged(GameState state)
        {
            if (state != GameState.Initialization) return;
            GenerateTerrain();
            gameStateManager.SetGameState(GameState.TerrainGenerated);
        }
        
        /// <summary>
        /// This method is used to generate the terrain
        /// </summary>
        private void OnValidate()
        {
            if (gameStateManager == null) gameStateManager = GetComponent<GameStateManager>();
        }
    }
}