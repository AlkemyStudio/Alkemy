using System.Collections.Generic;
using Bonus;
using Core;
using Game;
using Terrain;
using UnityEngine;

namespace Wall
{
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
        
        public void DestroyWallWithoutEffect()
        {
            Vector2Int tilePos = TerrainUtils.GetTilePosition(transform.position);
            TerrainManager.Instance.InstantiateFloor(tilePos.x, tilePos.y);
            Destroy(gameObject);
        }

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
                        Debug.Log($"Spawning bonus : {bonusToSpawn}");
                        BaseBonus bonus = Instantiate(
                            original: bonusToSpawn, 
                            position: transform.position + new Vector3(0, 0.5f, 0), 
                            rotation: Quaternion.Euler(0, 90, 90)
                        );
                        bonus.SetupBonus();
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
        
        private bool ShouldSpawnBonus()
        {
            return Random.Range(0, 101) < percentageToDropBonus;
        }

        private void OnDestroy()
        {
            entityHealth.OnDeath -= OnDeath;
        }

        private void OnValidate()
        {
            entityHealth = GetComponent<EntityHealth>();
        }
    }
}