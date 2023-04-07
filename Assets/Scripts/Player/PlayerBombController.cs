using System;
using Bomb;
using Lobby;
using Terrain;
using UnityEngine;

namespace Player
{
    public class PlayerBombController : MonoBehaviour
    {
        [SerializeField] private BaseBombController bombPrefab;

        [SerializeField] private int bombPower = 1;
        [SerializeField] private int minBombPower = 1;
        [SerializeField] private int maxBombPower = 9;
        
        [SerializeField] private int bombAmount = 1;
        
        [Header("Layer Masks Settings")]
        [SerializeField] private LayerMask bombLayerMask;

        private int _remainingBombs;
        private PlayerInputHandler _playerInputHandler;

        public int BombPower => bombPower;
        
        private void Start()
        {
            _remainingBombs = bombAmount;
        }
        
        public void Initialize(PlayerInputHandler playerInputHandler)
        {
            _playerInputHandler = playerInputHandler;
            _playerInputHandler.OnPlaceBomb += TryPlaceBomb;
        }

        private void OnDestroy()
        {
            _playerInputHandler.OnPlaceBomb -= TryPlaceBomb;
        }

        public void TryPlaceBomb()
        {
            if (_remainingBombs <= 0) return;

            Vector3 playerTilePosition = TerrainUtils.GetTerrainPosition(transform.position);

            bool canBePlaced = PlayerCanPlaceBombAt(playerTilePosition);
            if (!canBePlaced) return;
            
            PlaceBombAt(playerTilePosition);
        }
        
        private bool PlayerCanPlaceBombAt(Vector3 tilePosition)
        {
            return !Physics.CheckBox(tilePosition, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, bombLayerMask);
        }

        private void PlaceBombAt(Vector3 tilePosition)
        {
            Vector3 bombPosition = tilePosition;
            bombPosition.y = 0.376f;
            
            _remainingBombs--;
            BaseBombController bombController = Instantiate(bombPrefab, bombPosition, Quaternion.identity);
            bombController.SetupBomb(this, bombPower);
        }
        
        public void OnBombExplode()
        {
            _remainingBombs++;
        }
        
        public void AddBombPower()
        {
            if (bombPower < maxBombPower)
            {
                bombPower++;
            }
        }

        public void RemoveBombPower()
        {
            if (bombPower > minBombPower) {
                bombPower--;
            }
        }
        
        public void AddBombAmount()
        {
            bombAmount++;
            _remainingBombs++;
        }

        public void RemoveBombAmount()
        {
            if (bombAmount > 1)
            {
                bombAmount--;
            }

            if (_remainingBombs > bombAmount)
            {
                _remainingBombs = bombAmount;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Bomb"))
            {
                other.isTrigger = false;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (minBombPower <= 0) minBombPower = 1;
            if (maxBombPower < minBombPower) maxBombPower = minBombPower;
            bombPower = Mathf.Clamp(bombPower, minBombPower, maxBombPower);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(TerrainUtils.GetTerrainPosition(transform.position), new Vector3(0.5F, 0.5F, 0.5F));
        }
#endif
    }
}