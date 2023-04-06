using System.Collections.Generic;
using Bonus;
using Character;
using Terrain;
using UnityEngine;

namespace Wall
{
    [RequireComponent(typeof(EntityHealth))]
    public class DestructibleWall : MonoBehaviour
    {
        [SerializeField] private EntityHealth entityHealth;
        [SerializeField] private float percentageToDropBonus;
        [SerializeField] private List<BaseBonus> bonusPrefabs;

        private void Start()
        {
            entityHealth = entityHealth.GetComponent<EntityHealth>();
            entityHealth.OnDeath += OnDeath;
        }

        private void OnDeath(GameObject go)
        { 
            Vector2Int tilePos = TerrainUtils.GetTilePosition(transform.position);
            TerrainManager.Instance.InstantiateFloor(tilePos.x, tilePos.y);
            
            if (ShouldSpawnBonus())
            {
                BaseBonus bonus = Instantiate(
                    original: bonusPrefabs[Random.Range(0, bonusPrefabs.Count)], 
                    position: transform.position + new Vector3(0, 0.5f, 0), 
                    rotation: Quaternion.Euler(0, 90, 90)
                );
                bonus.SetupBonus();
            }

            Destroy(go);
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