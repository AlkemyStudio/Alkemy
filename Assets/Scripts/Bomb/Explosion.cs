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
        
        public void SetupExplosionBoundingBox(float distance, Vector3 direction) {
            Vector3 newSize = distance * direction;
            boxCollider.size = new Vector3(Math.Abs(newSize.x), Math.Abs(newSize.y), Math.Abs(newSize.z)) + new Vector3(0.7f, 1f, 0.7f);
            boxCollider.center = newSize / 2;
        }

        private void OnValidate() {
            if (boxCollider == null) {
                boxCollider = GetComponent<BoxCollider>();
            }
        }

        public void DestroyAfter(float time)
        {
            Destroy(gameObject, time);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<EntityHealth>().PerformDamage();
            }
        }
    }
}