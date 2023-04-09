using Player;
using UnityEngine;

namespace Bonus
{
    public class DecreaseSpeedBonus : BaseBonus
    {
        /// <summary>
        /// Decreases the speed of the player
        /// </summary>
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            player.GetComponent<PlayerMovement>().RemoveSpeedBonus();
        }
    }
}