using System.Collections.Generic;
using Bonus;
using Core;
using Game;
using Terrain;
using UnityEngine;
using VoxelMesher;

namespace Wall
{
    /// <summary>
    /// This class is used to manage the destructible wall
    /// </summary>
    [RequireComponent(typeof(EntityHealth), typeof(VoxelParser))]
    public class DestructibleWall : MonoBehaviour
    {
        [SerializeField] private EntityHealth entityHealth;
        [SerializeField] private float percentageToDropBonus;

        [SerializeField] private GameObject explosionPrefab;

        private VoxelParser voxelParser;
        private MeshRenderer meshRenderer;

        private bool isDead = false;
        
        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            entityHealth = entityHealth.GetComponent<EntityHealth>();
            entityHealth.OnDeath += OnDeath;
            voxelParser = GetComponent<VoxelParser>();
        }
        
        /// <summary>
        /// This method is used to destroy the wall without any effect
        /// </summary>
        public void DestroyWallWithoutEffect()
        {
            Vector2Int tilePos = TerrainUtils.GetTilePosition(transform.position);
            TerrainManager.Instance.InstantiateFloor(tilePos.x, tilePos.y);
            Destroy(gameObject);
        }

        /// <summary>
        /// This method is used to destroy the wall with an explosion effect
        /// </summary>
        /// <param name="go"> The game object that will be destroyed </param>
        private void OnDeath(GameObject go)
        { 
            if (isDead) return;
            isDead = true;

            Vector2Int tilePos = TerrainUtils.GetTilePosition(transform.position);
            TerrainManager.Instance.InstantiateFloor(tilePos.x, tilePos.y);
            
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            meshRenderer.enabled = false;

            VoxelGraph voxelGraph = explosion.GetComponent<VoxelGraph>();
            voxelGraph.voxelParser = voxelParser;
            voxelGraph.OnAnimationEnd += () => {
                if (ShouldSpawnBonus())
                {
                    try {
                        BaseBonus bonusToSpawn = GameBonuses.Instance.GetRandomBonus();
                        BaseBonus bonus = Instantiate(
                            original: bonusToSpawn, 
                            position: transform.position + new Vector3(0, 0.5f, 0), 
                            rotation: Quaternion.Euler(0, 90, 90)
                        );
                    } catch {
                    }
                }

                try {
                    Destroy(go);
                } catch {

                }
            };
            voxelGraph.enabled = true;
        }
        
        /// <summary>
        /// This method is used to check if the wall should spawn a bonus
        /// </summary>
        /// <returns> True if the wall should spawn a bonus, false otherwise </returns>
        private bool ShouldSpawnBonus()
        {
            return Random.Range(0, 101) < percentageToDropBonus;
        }

        /// <summary>
        /// This method is used to unsubscribe from the OnDeath event
        /// </summary>
        private void OnDestroy()
        {
            entityHealth.OnDeath -= OnDeath;
        }

        /// <summary>
        /// This method is used to validate the component
        /// </summary>
        private void OnValidate()
        {
            entityHealth = GetComponent<EntityHealth>();
        }
    }
}