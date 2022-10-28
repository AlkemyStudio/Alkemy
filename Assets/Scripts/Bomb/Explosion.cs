using System;
using Character;
using UnityEngine;

namespace Bomb
{
    public class Explosion : MonoBehaviour
    {
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