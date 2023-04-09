using System;
using Core;
using Terrain;
using UnityEngine;

namespace Bomb
{
    [RequireComponent(typeof(BoxCollider))]
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private BoxCollider boxCollider;
        
        // Sets the box collider size and position for an explosion
        // based on the explosion's distance and direction.
        public void SetupExplosionBoundingBox(float distance, Vector3 direction) {
            Vector3 newSize = distance * direction;
            boxCollider.size = new Vector3(Math.Abs(newSize.x), Math.Abs(newSize.y), Math.Abs(newSize.z)) + new Vector3(0.7f, 1f, 0.7f);
            boxCollider.center = newSize / 2;
        }



        // If the BoxCollider component is not set, attempt to find it.

        private void OnValidate() {
            if (boxCollider == null) {
                boxCollider = GetComponent<BoxCollider>();
            }
        }

        // This function destroys the game object after the specified time.
        // The game object is destroyed using the Destroy function.
        public void DestroyAfter(float time)
        {
            Destroy(gameObject, time);
        }



        //This code is used to detect when the player is in range of the enemy.
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<EntityHealth>().PerformDamage();
            }
        }
    }
}