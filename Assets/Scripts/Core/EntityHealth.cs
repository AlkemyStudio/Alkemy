using Audio;
using UnityEngine;

namespace Core
{
    public class EntityHealth : MonoBehaviour
    {
        [SerializeField] private AudioClip deathSound;
        private bool isDeathSoundNotNull;

        public delegate void HealthAction(GameObject go);

        public event HealthAction OnDeath;

        private void Start()
        {
            isDeathSoundNotNull = deathSound != null;
        }

        public void PerformDamage()
        {
            if (isDeathSoundNotNull)
            {
                AudioSourcePool.Instance.PlayClipAtPoint(deathSound, transform.position);
            }
            
            OnDeath?.Invoke(gameObject);
        }
    }
}