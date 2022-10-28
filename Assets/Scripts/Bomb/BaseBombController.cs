using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using Terrain;
using UnityEngine;

namespace Bomb
{
    public abstract class BaseBombController : MonoBehaviour
    {
        [SerializeField] MeshRenderer meshRenderer;
        
        private BombData _bombData;
        private TerrainManager _terrainManager;
        private bool _isExploded;
        private bool _isExploding;

        private readonly Vector3 SouthRayDirection = new Vector3(0, 0.5f, -1);
        private readonly Vector3 NorthRayDirection = new Vector3(0, 0.5f, 1);
        private readonly Vector3 WestRayDirection = new Vector3(-1, 0.5f, 0);
        private readonly Vector3 EastRayDirection = new Vector3(1, 0.5f, 0);

        public virtual void SetupBomb(BombData bombData, TerrainManager terrainManager)
        {
            _bombData = bombData;
            _terrainManager = terrainManager;
            StartCoroutine(StartCountdown());
        }

        private void Explode()
        {
            StopCoroutine(StartCountdown());
            _isExploded = true;
            StartCoroutine(ExplodeCoroutine());
        }

        protected virtual IEnumerator StartCountdown()
        {
            yield return new WaitForSeconds(BombData.CountdownDuration);
            Explode();
        }
        
        private IEnumerator ExplodeCoroutine()
        {
            _isExploding = true;
            yield return new WaitForSeconds(BombData.ExplosionDuration);
            _isExploding = false;
        }

        private void FixedUpdate()
        {
            if (_isExploded && _isExploding)
            {
                Explooooooosionnn();
            }
        }

        // ReSharper disable once IdentifierTypo
        protected virtual void Explooooooosionnn()
        {
            Vector2Int bombGridPosition = _terrainManager.GetTilePosition(transform.position);
            _terrainManager.SetValue(bombGridPosition, TerrainEntityType.None);
            meshRenderer.enabled = false;

            List<GameObject> hitEntities = GetHitEntitiesToDestroy(bombGridPosition);
            foreach (GameObject hitEntity in hitEntities)
            {
                if (hitEntity.CompareTag("Player")) {
                    hitEntity.GetComponent<EntityHealth>()?.PerformDamage();
                } else if (hitEntity.CompareTag("Bomb")) {
                    hitEntity.GetComponent<BaseBombController>()?.Explode();
                } else if (hitEntity.CompareTag("Destructible")) {
                    _terrainManager.SetValue(_terrainManager.GetTilePosition(transform.position), TerrainEntityType.None);
                    hitEntity.GetComponent<EntityHealth>()?.PerformDamage();
                }
            }
            
            Destroy(gameObject);
        }

        protected virtual List<GameObject> GetHitEntitiesToDestroy(Vector2Int bombGridPosition)
        {
            Vector3 bombPosition = new Vector3(bombGridPosition.x, 0.5f, bombGridPosition.y);
            RaycastHit[] souRaycastHits = Physics.RaycastAll(bombPosition, SouthRayDirection, _bombData.power);
            RaycastHit[] norRaycastHits = Physics.RaycastAll(bombPosition, NorthRayDirection, _bombData.power);
            RaycastHit[] wesRaycastHits = Physics.RaycastAll(bombPosition, WestRayDirection, _bombData.power);
            RaycastHit[] easRaycastHits = Physics.RaycastAll(bombPosition, EastRayDirection, _bombData.power);

            List<GameObject> hitEntities = new List<GameObject>();

            hitEntities.AddRange(ComputeHitEntities(souRaycastHits));
            hitEntities.AddRange(ComputeHitEntities(norRaycastHits));
            hitEntities.AddRange(ComputeHitEntities(wesRaycastHits));
            hitEntities.AddRange(ComputeHitEntities(easRaycastHits));

            return hitEntities;
        }

        private List<GameObject> ComputeHitEntities(RaycastHit[] raycastHits)
        {
            bool foundDestructible = false;
            GameObject firstDestructible = null;
            float lastWallDistance = Mathf.Infinity;

            List<GameObject> hitEntities = new List<GameObject>();
            foreach (RaycastHit raycastHit in raycastHits)
            {
                if (raycastHit.collider.gameObject.CompareTag("Player") ||
                    raycastHit.collider.gameObject.CompareTag("Bomb"))
                {
                    hitEntities.Add(raycastHit.collider.gameObject);
                    continue;
                }

                if (!raycastHit.collider.gameObject.CompareTag("Destructible")) continue;

                if (!foundDestructible)
                {
                    firstDestructible = raycastHit.collider.gameObject;
                    foundDestructible = true;
                }
                else if (Vector3.Distance(firstDestructible.transform.position, raycastHit.collider.transform.position) <
                         lastWallDistance)
                {
                    firstDestructible = raycastHit.collider.gameObject;
                    lastWallDistance = Vector3.Distance(firstDestructible.transform.position,
                        raycastHit.collider.transform.position);
                }
            }

            if (foundDestructible)
            {
                hitEntities.Add(firstDestructible);
            }

            return hitEntities;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            
            Vector2Int bombGridPosition = _terrainManager.GetTilePosition(transform.position); 
            Vector3 _bombPosition = new Vector3(bombGridPosition.x, 0.5f, bombGridPosition.y);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_bombPosition, SouthRayDirection * _bombData.power);
            Gizmos.DrawRay(_bombPosition, NorthRayDirection * _bombData.power);
            Gizmos.DrawRay(_bombPosition, WestRayDirection * _bombData.power);
            Gizmos.DrawRay(_bombPosition, EastRayDirection * _bombData.power);
        }
    }
}