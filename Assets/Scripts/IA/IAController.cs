using System;
using UnityEngine;

namespace IA
{
    public class IAController : MonoBehaviour
    {
        [SerializeField] private float baseFailingRate;
        [SerializeField] private float overTimefailingRateFactor;
        
        private bool _shouldUpdateInput = false;
        private PlayerMovement _playerMovement;
        private PlayerBombController _playerBombController;

        void Start()
        {
            EntityHealth entityHealth = player.GetComponent<EntityHealth>();
            entityHealth.OnDeath += OnDeath;
            
            _playerMovement = player.GetComponent<PlayerMovement>();
            _playerBombController = player.GetComponent<PlayerBombController>();
        }

        void Update()
        {
            if (!_shouldUpdateInput) return;
            
            RaycastResult raycastResult = GetBombPositions();
            bool shouldPlaceBomb = ShouldPlaceBomb(raycastResult);
            if (shouldPlaceBomb)
            {
                PlaceBomb();
            }
            
            MovementDirection movementDirection = GetMovementDirection(raycastResult, shouldPlaceBomb);
            if (movementDirection != MovementDirection.None)
            {
                Move(movementDirection.ToVector2());
            }
        }
        
        RaycastResult GetBombPositions(Vector2 playerPosition)
        {
            
        }

        bool ShouldPlaceBomb(Vector2 playerPosition, RaycastResult raycastResult)
        {
            if (raycastResult.ContainsBomb()) return false;

            if (GetEscapeDirection() == MovementDirection.None) return false;
            
            return IsWallAndInExplosionRange(raycastResult.top)
                || IsWallAndInExplosionRange(raycastResult.right)
                || IsWallAndInExplosionRange(raycastResult.bottom)
                || IsWallAndInExplosionRange(raycastResult.left);
        }

        bool IsWallAndInExplosionRange(TouchData data)
        {
            return data.IsWall && playerPosition.Distance(data.Position) <= _playerBombController.BombPower);
        }

        MovementDirection GetMovementDirection(Vector2 playerPosition, RaycastResult raycastResult, bool shouldPlaceBomb)
        {
            return IsNoneAndOutOfExplosionRange(raycastResult.top) 
                || IsNoneAndOutOfExplosionRange(raycastResult.right)
                || IsNoneAndOutOfExplosionRange(raycastResult.bottom)
                || IsNoneAndOutOfExplosionRange(raycastResult.left);
        }

        bool IsNoneAndOutOfExplosionRange(TouchData data)
        {
            return !data.IsWall 
                && !data.IsBomb 
                && !data.IsTerrain
                && playerPosition.Distance(data.Position) > _playerBombController.BombPower;
        }
        
        MovementDirection GetEscapeDirection(Vector2 playerPosition, RaycastResult raycastResult)
        {
            return MovementDirection.None;
        }

        private void Move(Vector2 inputVector)
        {
            _playerMovement.UpdateInputVector(inputVector);
        }
        
        private void PlaceBomb()
        {
            _playerBombController.TryPlaceBomb();
        }

        private void OnDeath(GameObject player)
        {
            _shouldUpdateInput = false;
        }
    }
}