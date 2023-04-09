using System;
using Bomb;
using Lobby;
using PlayerInputs;
using Terrain;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// PlayerController is used to control the bombs of the player.
    /// </summary>
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
        
        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        private void Start()
        {
            _remainingBombs = bombAmount;
        }
        
        /// <summary>
        /// Initialize the player controller.
        /// </summary>
        /// <param name="playerInputHandler"> The player input handler. </param>
        public void Initialize(PlayerInputHandler playerInputHandler)
        {
            _playerInputHandler = playerInputHandler;
            _playerInputHandler.OnPlaceBomb += TryPlaceBomb;
        }

        /// <summary>
        /// OnDestroy is called when the script instance is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            _playerInputHandler.OnPlaceBomb -= TryPlaceBomb;
        }

        /// <summary>
        /// Try to place a bomb.
        /// </summary>
        public void TryPlaceBomb()
        {
            if (_remainingBombs <= 0) return;

            Vector3 playerTilePosition = TerrainUtils.GetTerrainPosition(transform.position);

            bool canBePlaced = PlayerCanPlaceBombAt(playerTilePosition);
            if (!canBePlaced) return;
            
            PlaceBombAt(playerTilePosition);
        }
        
        /// <summary>
        /// PlayerCanPlaceBombAt is used to check if the player can place a bomb at the given tile position.
        /// </summary>
        /// <param name="tilePosition"></param>
        /// <returns></returns>
        private bool PlayerCanPlaceBombAt(Vector3 tilePosition)
        {
            return !Physics.CheckBox(tilePosition, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, bombLayerMask);
        }

        /// <summary>
        /// Place a bomb at the given tile position.
        /// </summary>
        /// <param name="tilePosition"> The tile position. </param>
        private void PlaceBombAt(Vector3 tilePosition)
        {
            Vector3 bombPosition = tilePosition;
            bombPosition.y = 0.376f;
            
            _remainingBombs--;
            BaseBombController bombController = Instantiate(bombPrefab, bombPosition, Quaternion.identity);
            bombController.SetupBomb(this, bombPower);
        }
        
        /// <summary>
        /// OnBombExplode is called when a bomb explodes.
        /// </summary>
        public void OnBombExplode()
        {
            _remainingBombs++;
        }
        
        /// <summary>
        /// Add bomb power.
        /// </summary>
        public void AddBombPower()
        {
            if (bombPower < maxBombPower)
            {
                bombPower++;
            }
        }

        /// <summary>
        /// Remove bomb power.
        /// </summary>
        public void RemoveBombPower()
        {
            if (bombPower > minBombPower) {
                bombPower--;
            }
        }
        
        /// <summary>
        /// Add bomb amount.
        /// </summary>
        public void AddBombAmount()
        {
            bombAmount++;
            _remainingBombs++;
        }

        /// <summary>
        /// Remove bomb amount.
        /// </summary>
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
        
        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Bomb"))
            {
                other.GetComponent<BaseBombController>().OnPlayerWalkOutsideTrigger();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (minBombPower <= 0) minBombPower = 1;
            if (maxBombPower < minBombPower) maxBombPower = minBombPower;
            bombPower = Mathf.Clamp(bombPower, minBombPower, maxBombPower);
        }
        
        /// <summary>
        /// OnDrawGizmos is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(TerrainUtils.GetTerrainPosition(transform.position), new Vector3(0.5F, 0.5F, 0.5F));
        }
#endif
    }
}