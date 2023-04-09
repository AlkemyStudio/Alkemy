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
        
        //This function adds a force to the bomb's rigidbody in the direction of the player
        //It is called in the BombExplode.cs script when the bomb is pushed by the player
        //The bombPushingSpeed is a public variable of the BombExplode.cs script

        protected virtual void Push(Vector3 playerDirection)
        {
            Vector3 velocity = new Vector3(playerDirection.x, 0, playerDirection.z) * bombPushingSpeed;
            bombRigidbody.AddForce(velocity, ForceMode.Impulse);
        }

        
        // This function stops the bomb's movement and sets its position to a whole number
        // so that it is on the grid. It also sets the last push time to the current time.
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

        
        // This function will be called when the player is stopping the bomb
        // or when the player is pushing the bomb
        // If the player has the effect to push the bomb, he can push the bomb in the same direction that he is moving
        // If the player has the effect to stop the bomb, he can stop the bomb
        // If the player is pushing the bomb, he can't push the bomb again during a delay
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

        
        //This function checks if the bomb is moving in the same direction as the object it is colliding with
        //It takes two vectors, the other object's position and the bomb's position
        //It returns a boolean, true if the bomb is moving in the same direction as the other object
        //It is used to check if a bomb is colliding with another object and moving in the same direction as it

        private bool IsMovingInTheSameDirection(Vector3 otherPosition, Vector3 position)
        {
            Vector3 direction = otherPosition - position;
            direction.Normalize();
            
            Vector3 rgbVelocity = bombRigidbody.velocity;
            rgbVelocity.Normalize();
            
            return Vector3.Dot(rgbVelocity, direction) > 0.96;
        }
        
        // This function computes the direction from position to otherPosition.
        // The result is a vector with a magnitude of 1.
        // If the X component of the result is 1, then the direction is to the right.
        // If the X component of the result is -1, then the direction is to the left.
        // If the Z component of the result is 1, then the direction is forward.
        // If the Z component of the result is -1, then the direction is backward.
        // If the X and Z components of the result are 0, then the direction is up.
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
            OnTouchSomething(other);
        }
    }
}
