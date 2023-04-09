using Player;
using UnityEngine;

namespace Bonus
{
    public class IncreaseSpeedBonus : BaseBonus
    {
        /// <summary>
        /// Increases the speed of the player
        /// </summary>
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            player.GetComponent<PlayerMovement>().AddSpeedBonus();
        }
    }
}