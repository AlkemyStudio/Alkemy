using System;
using System.Collections;
using Audio;
using Core;
using Game;
using Player;
using Terrain;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace Bomb
{
    public abstract class BaseBombController : MonoBehaviour
    {
        [Header("References")]
        [FormerlySerializedAs("colliderThatApplyForces")] [SerializeField] protected Collider mainCollider;
        [SerializeField] protected Collider colliderThatBlockForces;
        [SerializeField] protected Collider triggerCollider;
        [SerializeField] protected MeshRenderer meshRenderer;
        [SerializeField] protected Explosion explosionPrefabs;
        [SerializeField] protected AudioClip[] bombExplosionSounds;
        
        [Header("Settings")]
        [SerializeField] protected float fuseTime = 3.0f;
        
        protected PlayerBombController _playerBombController;
        protected int _bombPower;
        
        protected bool _hasAlreadyExploded = false;
        protected float spawnTime;
        
        protected static readonly int FuseTime = Shader.PropertyToID("_FuseTime");
        protected static readonly int SpawnTime = Shader.PropertyToID("_SpawnTime");

        private void Awake()
        {
            Physics.IgnoreCollision(mainCollider, colliderThatBlockForces, true);
            Physics.IgnoreCollision(mainCollider, triggerCollider, true);
            Physics.IgnoreCollision(colliderThatBlockForces, triggerCollider, true);
        }

        private void Start()
        {
            spawnTime = Time.time;
            meshRenderer.material = new Material(meshRenderer.material);
            meshRenderer.sharedMaterial.SetFloat(FuseTime, fuseTime);
            meshRenderer.sharedMaterial.SetFloat(SpawnTime, spawnTime);
        }

        public virtual void SetupBomb(PlayerBombController bombController, int bombPower)
        {
            _playerBombController = bombController;
            _bombPower = bombPower;
            GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
            StartCoroutine(StartTimer());
        }
        
        private void OnGameStateChanged(GameState gameState)
        {
            if (gameState == GameState.Ended)
            {
                Destroy(gameObject);
            }
        }

        protected virtual IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(fuseTime);
            StartExplode();
        }
        
        public virtual void CancelTimer()
        {
            StopAllCoroutines();
        }

        public virtual bool HasAlreadyExploded()
        {
            return _hasAlreadyExploded;
        }
        
        public virtual void StartExplode()
        {
            _hasAlreadyExploded = true;
            Vector3 bombPosition = TerrainUtils.GetTerrainPosition(transform.position);
            
            Explode(bombPosition, Vector3.back, _bombPower);
            Explode(bombPosition, Vector3.right, _bombPower);
            Explode(bombPosition, Vector3.forward, _bombPower);
            Explode(bombPosition, Vector3.left, _bombPower);
            
            if (_playerBombController != null)
            {
                _playerBombController.OnBombExplode();
            }
            
            PlaySound();
            Destroy(gameObject);
        }

        protected virtual void Explode(Vector3 position, Vector3 direction, int length)
        {
            if (length <= 0) return;

            float computedLength = 0;
            bool earlyExit = false;
            bool isBuilding = false;

            for (int i = 0; i < length; i++) {
                Vector3 newPosition = position + direction * (i+1);
                computedLength = i+1;
                Collider[] colliders = Physics.OverlapBox(newPosition, new Vector3(0.49f, 0.49f, 0.49f), Quaternion.identity);

                foreach (Collider c in colliders)
                {
                    if (c.CompareTag("Terrain"))
                    {
                        earlyExit = true;
                        break;
                    }
                    
                    if (c.CompareTag("Bomb"))
                    {
                        BaseBombController bombController = c.GetComponent<BaseBombController>();
                        if (bombController.HasAlreadyExploded())
                            continue;
                        bombController.CancelTimer();
                        bombController.StartExplode();
                        break;
                    }

                    if (c.CompareTag("Destructible"))
                    {
                        EntityHealth entityHealth = c.GetComponent<EntityHealth>();
                        entityHealth.PerformDamage();
                        earlyExit = true;
                        isBuilding = true;
                        break;
                    }
                }

                if (earlyExit) break;
            }

            Explosion explosion = Instantiate(explosionPrefabs, position, Quaternion.identity);
            VisualEffect vfx = explosion.GetComponent<VisualEffect>();
            explosion.SetupExplosionBoundingBox(computedLength - (earlyExit ? 1 : 0), direction);
            VoxelDeflagrationController deflagrationController = explosion.GetComponent<VoxelDeflagrationController>();
            vfx.SetVector3("Direction", direction);
            if (earlyExit && !isBuilding) {
                // If the explosion is not on a destructible wall, we need to reduce the length by 0.5f
                // to avoid the explosion to go through the wall
                computedLength -= 0.5f;
            }
            if (!earlyExit) {
                // If the explosion is not on a destructible wall or a terrain, we need to increase the length by 0.5f
                // to avoid the explosion to stop before the end of the wall
                computedLength += 0.5f;
            }
            vfx.SetFloat("Max Distance", computedLength);
            deflagrationController.Play();
        }

        protected virtual void PlaySound()
        {
            if (bombExplosionSounds.Length == 0) return;
            int randomIndex = Random.Range(0, bombExplosionSounds.Length);
            AudioSourcePool.Instance.PlayClipAtPoint(bombExplosionSounds[randomIndex], transform.position);
        }
        
        public virtual void OnPlayerWalkOutsideTrigger()
        {
            mainCollider.isTrigger = false;
            colliderThatBlockForces.isTrigger = false;
            triggerCollider.enabled = true;
        }
        
        private void OnDestroy()
        {
            GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}