using System;
using UnityEngine;

namespace Character
{
    public class EntityHealth : MonoBehaviour
    {
        public delegate void HealthAction(GameObject gameObject);
        public event HealthAction OnDeath;

        public void PerformDamage()
        {
            OnDeath?.Invoke(gameObject);
        }
    }
}