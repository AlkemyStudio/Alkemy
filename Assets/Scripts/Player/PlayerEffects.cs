using UnityEngine;

namespace Player
{
    public class PlayerEffects : MonoBehaviour
    {
        [SerializeField] private bool hasPushBombEffect = false;
        
        public bool HasPushBombEffect => hasPushBombEffect;
        
        public void SetPushBombEffect(bool hasPushBombEffect)
        {
            this.hasPushBombEffect = hasPushBombEffect;
        }
    }
}