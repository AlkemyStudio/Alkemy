using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody playerRigidbody;
        
        [Header("Speed Settings")]
        [SerializeField] private float baseSpeed = 4.0f;
        [SerializeField] private float bonusSpeed = 1.0f;
        
        [Header("Bonus Settings")]
        [SerializeField] private int bonusNumber = 1;
        [SerializeField] private int minBonusNumber = 1;
        [SerializeField] private int maxBonusNumber = 9;
        
        
        private float _speed;
        private Vector2 _inputVector = Vector2.zero;
        private GameInputController _gameInputController;

        private void OnEnable()
        {
            _gameInputController = new GameInputController();
            _gameInputController.Player.Move.Enable();
        }

        private void OnDisable()
        {
            _gameInputController.Player.Move.Disable();
        }

        private void Start()
        {
            ComputeSpeed();
        }

        public void AddSpeedBonus()
        {
            bonusNumber++;
            ComputeSpeed();
        }
        
        public void RemoveSpeedBonus()
        {
            if (bonusNumber <= 1) return;
            bonusNumber--;
            ComputeSpeed();
        }
        
        private void Update()
        {
            _inputVector = _gameInputController.Player.Move.ReadValue<Vector2>();
        }

        public void FixedUpdate()
        {
            Vector3 translation = new Vector3(_inputVector.x, 0, _inputVector.y) * (_speed * Time.fixedDeltaTime);
            playerRigidbody.MovePosition(playerRigidbody.position + translation);
        }

        private void ComputeSpeed()
        {
            _speed = baseSpeed + bonusSpeed * bonusNumber;
        }

        private void OnValidate()
        {
            if (playerRigidbody == null) playerRigidbody = GetComponent<Rigidbody>();
            
            if (minBonusNumber < 1) minBonusNumber = 1;
            if (maxBonusNumber < minBonusNumber) maxBonusNumber = minBonusNumber;
            bonusNumber = Mathf.Clamp(bonusNumber, minBonusNumber, maxBonusNumber);
            ComputeSpeed();
        }
    }
}