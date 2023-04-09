using UnityEngine;

namespace Player
{
    /// <summary>
    /// PlayerEffects is used to store the effects of the player.
    /// </summary>
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