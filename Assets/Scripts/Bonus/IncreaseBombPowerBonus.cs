using Player;
using UnityEngine;

namespace Bonus
{
    public class IncreaseBombPowerBonus : BaseBonus
    {
        /// <summary>
        /// Increases the bomb power of the player
        /// </summary>
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            var bombController = player.GetComponent<PlayerBombController>();
            bombController.AddBombPower();
        }
    }
}