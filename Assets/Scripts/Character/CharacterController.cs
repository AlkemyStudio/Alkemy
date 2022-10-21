using Bomb;
using Game;
using Terrain;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private CharacterState characterState;
        [SerializeField] private Rigidbody playerRigidbody;

        private Transform _playerTransform;
        private TerrainManager _terrainManager;
        private GameInputController _gameInputController;
        private Vector2 _inputVector = Vector2.zero;
        private bool _shouldUpdate = true;
        
        private void Awake()
        {
            _playerTransform = transform;
        }

        private void Start()
        {
            _terrainManager = TerrainManager.Instance;
        }

        private void FixedUpdate()
        {
            if (!_shouldUpdate)
            {
                playerRigidbody.velocity = Vector3.zero;
                return;
            }
            
            playerRigidbody.velocity = new Vector3(_inputVector.x, 0, _inputVector.y) * (characterState.speed * Time.deltaTime);
        }

        private void Update()
        {
            if (!_shouldUpdate) return;
            
            _inputVector = _gameInputController.Player.Move.ReadValue<Vector2>();
            bool isPlacingBombIsPress = _gameInputController.Player.PlaceBomb.triggered;

            if (!isPlacingBombIsPress) return;
            
            Vector2Int tilePos = _terrainManager.GetTilePosition(_playerTransform.position);
            bool canBePlace = BombCanBePlaced(tilePos);
            if (canBePlace)
            {
                PlaceBomb(tilePos);
            }
        }

        private bool BombCanBePlaced(Vector2Int tilePos)
        {
            return _terrainManager.IsFilled(tilePos.x, tilePos.y);
        }
        
        private void PlaceBomb(Vector2Int tilePos)
        {
            _terrainManager.SetValue(tilePos.x, tilePos.y, TerrainEntityType.Bomb);
            GameObject bomb = Instantiate(characterState.bombPrefabs, new Vector3(tilePos.x, 0.375f, tilePos.y), Quaternion.identity);
            bomb.GetComponent<BaseBombController>().SetupBomb(new BombData(characterState.bombPower), _terrainManager);
        }

        private void OnEnable()
        {
            _gameInputController = new GameInputController();
            _gameInputController.Player.Move.Enable();
            _gameInputController.Player.PlaceBomb.Enable();

            GameManager.Instance.GameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            _gameInputController.Player.Move.Enable();
            _gameInputController.Player.PlaceBomb.Enable();
            _gameInputController.Dispose();

            GameManager.Instance.GameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState state)
        {
            _shouldUpdate = state == GameState.Playing;
            Debug.Log("Player Enabled: " + _shouldUpdate);
        }

        private void OnValidate()
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }
    }
}