using System;
using Player;
using UnityEngine;

namespace Bomb
{
    public class BasePushBombController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected Rigidbody bombRigidbody;
        
        [Header("Settings")]
        [SerializeField] protected float delayAfterStoppingTheBomb = 0.1f;
        [SerializeField] protected float bombPushingSpeed = 10.0f;
        
        protected float _lastPushTime;
        protected Transform _parentTransform;

        private void Start()
        {
            _parentTransform = transform.parent;
        }

        protected virtual bool IsMoving() => bombRigidbody.velocity != Vector3.zero;
        
        protected virtual void Push(Vector3 playerDirection)
        {
            Vector3 velocity = new Vector3(playerDirection.x, 0, playerDirection.z) * bombPushingSpeed;
            bombRigidbody.AddForce(velocity, ForceMode.Impulse);
        }

        protected void StopMove()
        {
            bombRigidbody.velocity = Vector3.zero;
            
            Vector3 bombPosition = _parentTransform.position;
            bombPosition = new Vector3(
                Mathf.FloorToInt(bombPosition.x) + 0.5F, 
                bombPosition.y, 
                Mathf.FloorToInt(bombPosition.z) + 0.5F
            );
            _parentTransform.position = bombPosition;
            _lastPushTime = Time.time;
        }
        
        protected virtual void OnTouchSomething(Collider collision)
        {
            GameObject other = collision.gameObject;
            
            if (IsMoving() && !IsMovingInTheSameDirection(collision.transform.position, _parentTransform.position)) 
                return;
            
            if (other.CompareTag("Bonus")) 
                return;

            if (IsMoving())
            {
                Debug.Log("Stop moving");
                StopMove();
                return;
            }

            if (!other.CompareTag("Player")) 
                return;
            
            if (IsMoving()) 
                return;
                
            PlayerEffects playerEffects = other.GetComponent<PlayerEffects>();
                
            if (!playerEffects.HasPushBombEffect) 
                return;
            
            if (_lastPushTime + delayAfterStoppingTheBomb > Time.time) 
                return;
            
            Vector3 direction = ComputeDirection(_parentTransform.position, other.transform.position);
            Push(direction);
        }
        
        private bool IsMovingInTheSameDirection(Vector3 otherPosition, Vector3 position)
        {
            Vector3 direction = otherPosition - position;
            direction.Normalize();
            
            Vector3 rgbVelocity = bombRigidbody.velocity;
            rgbVelocity.Normalize();

            bool r = Mathf.RoundToInt(Vector3.Dot(rgbVelocity, direction)) == 1;
            return r;
        }
        
        private static Vector3 ComputeDirection(Vector3 position, Vector3 otherPosition)
        {
            Vector3 direction = position - otherPosition;
            
            direction.Normalize();

            float angleX = Vector3.Dot(direction, Vector3.right);
            float angleZ = Vector3.Dot(direction, Vector3.forward);
            
            float directionX = Mathf.Round(Mathf.Abs(angleX) > Mathf.Abs(angleZ) ? Mathf.Sign(angleX) : 0);
            float directionZ = Mathf.Round(Mathf.Abs(angleX) < Mathf.Abs(angleZ) ? Mathf.Sign(angleZ) : 0);

            Vector3 result = new Vector3(directionX, 0, directionZ);
            
            return result;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("[BaseBombController] OnTriggerEnter : " + other.gameObject.name);
            OnTouchSomething(other);
        }
    }
}
