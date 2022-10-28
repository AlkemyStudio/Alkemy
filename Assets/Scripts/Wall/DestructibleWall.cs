using System.Collections.Generic;
using Bonus;
using Character;
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
            entityHealth.OnDeath += OnDeath;
        }

        private void OnDeath(GameObject go)
        {
            if (ShouldSpawnBonus())
            {
                BaseBonus bonus = Instantiate(bonusPrefabs[Random.Range(0, bonusPrefabs.Count)], transform.position, Quaternion.identity);
                bonus.SetupBonus();
            }

            Destroy(go);
        }
        
        private bool ShouldSpawnBonus()
        {
            return Random.Range(0, 100) < percentageToDropBonus;
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