using Player;
using UnityEngine;

namespace Bonus
{
    public class DecreaseBombPowerBonus : BaseBonus
    {
        /// <summary>
        /// Decreases the bomb power of the player
        /// </summary>
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            var bombController = player.GetComponent<PlayerBombController>();
            bombController.RemoveBombPower();
        }
    }
}