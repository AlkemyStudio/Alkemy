using System;
using Lobby;
using UnityEngine;

namespace Player
{
    enum PlayerOrientation
    {
        Top,
        Right,
        Bottom,
        Left
    }
    
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody playerRigidbody;
        [SerializeField] private Transform modelTransform;
        
        [Header("Speed Settings")]
        [SerializeField] private float baseSpeed = 120.0f;
        [SerializeField] private float speedPerBonus = 60.0f;
        
        [Header("Bonus Settings")]
        [SerializeField] private int bonusNumber = 1;
        [SerializeField] private int minBonusNumber = 1;
        [SerializeField] private int maxBonusNumber = 9;


        private PlayerInputHandler _playerInputHandler;
        private Vector3 lookAtPosition = Vector3.zero;
        
        private float _speed;
        private Vector2 _inputVector = Vector2.zero;

        private void Start()
        {
            ComputeSpeed();
        }
        
        public void Initialize(PlayerInputHandler playerInputHandler)
        {
            _playerInputHandler = playerInputHandler;
            _playerInputHandler.OnMove += UpdateInputVector;
        }

        private void OnDestroy()
        {
            _playerInputHandler.OnMove -= UpdateInputVector;
        }

        public void AddSpeedBonus()
        {
            if (maxBonusNumber == bonusNumber) return;
            bonusNumber++;
            ComputeSpeed();
        }
        
        public void RemoveSpeedBonus()
        {
            if (bonusNumber <= minBonusNumber) return;
            bonusNumber--;
            ComputeSpeed();
        }

        public void UpdateInputVector(Vector2 inputVector)
        {
            _inputVector = inputVector;
        }

        public void FixedUpdate()
        {
            Vector3 translation = new Vector3(_inputVector.x, 0, _inputVector.y) * (_speed * Time.fixedDeltaTime);
            playerRigidbody.AddForce(translation);
            playerRigidbody.velocity = translation;

            if (_inputVector == Vector2.zero) return;
            
            Vector3 position = modelTransform.position;
            lookAtPosition = new Vector3(
                x: position.x + _inputVector.x * -1,
                y: position.y,
                z: position.z + _inputVector.y * -1
            );

            modelTransform.LookAt(lookAtPosition);
        }

        private void ComputeSpeed()
        {
            _speed = baseSpeed + speedPerBonus * bonusNumber;
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