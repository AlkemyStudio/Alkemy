using System.Collections;
using Bomb;
using UnityEngine;

namespace Bonus
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class BaseBonus : MonoBehaviour
    {
        [SerializeField] private Collider bonusCollider;

        public void SetupBonus()
        {
            StartCoroutine(EnableAfter(BombData.ExplosionDuration));
        }

        private IEnumerator EnableAfter(float duration)
        {
            yield return new WaitForSeconds(duration);
            bonusCollider.enabled = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerTakeBonus(other.gameObject);
            }
            
            Destroy(gameObject);
        }

        protected abstract void OnPlayerTakeBonus(GameObject player);

        private void OnValidate()
        {
            if (bonusCollider == null)
            {
                bonusCollider = GetComponent<Collider>();
            }
        }
    }
}