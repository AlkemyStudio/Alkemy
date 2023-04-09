using System.Collections;
using Audio;
using Bomb;
using UnityEngine;

namespace Bonus
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class BaseBonus : MonoBehaviour
    {
        [SerializeField] private Collider bonusCollider;
        [SerializeField] private Transform modelTransform;
        [SerializeField] private float rotationSpeed = 125.0f;
        [SerializeField] private AudioClip bonusSound;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlaySound();
                OnPlayerTakeBonus(other.gameObject);
            }
            
            Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            modelTransform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        }

        protected abstract void OnPlayerTakeBonus(GameObject player);
        
        protected virtual void PlaySound()
        {
            if (bonusSound != null)
            {
                AudioSourcePool.Instance.PlayClipAtPoint(bonusSound, transform.position);
            }
        }

        private void OnValidate()
        {
            if (bonusCollider == null)
            {
                bonusCollider = GetComponent<Collider>();
            }
        }
    }
}