using Bomb;
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
        private GameInputController _gameInputController;
        
        private void Start()
        {
            _remainingBombs = bombAmount;
        }

        private void Update()
        {
            if (!_gameInputController.Player.PlaceBomb.triggered) return;
            if (_remainingBombs <= 0) return;

            Vector3 playerTilePosition = TerrainUtils.GetTerrainPosition(transform.position);
            
            if (PlayerCanPlaceBombAt(playerTilePosition)) return;

            PlaceBombAt(playerTilePosition);
        }
        
        private bool PlayerCanPlaceBombAt(Vector3 tilePosition)
        {
            return Physics.CheckBox(tilePosition, Vector3.one / 2, Quaternion.identity, bombLayerMask);
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
        
        public void AddBombAmount()
        {
            bombAmount++;
            _remainingBombs++;
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Bomb"))
            {
                other.isTrigger = false;
            }
        }

        private void OnEnable()
        {
            _gameInputController = new GameInputController();
            _gameInputController.Player.PlaceBomb.Enable();
        }

        private void OnValidate()
        {
            if (minBombPower <= 0) minBombPower = 1;
            if (maxBombPower < minBombPower) maxBombPower = minBombPower;
            bombPower = Mathf.Clamp(bombPower, minBombPower, maxBombPower);
        }
    }
}