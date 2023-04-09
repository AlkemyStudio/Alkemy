using Player;
using UnityEngine;

namespace Bonus
{
    public class DecreaseBombAmountBonus : BaseBonus
    {
        /// <summary>
        /// Decreases the bomb amount of the player
        /// </summary>
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            var bombController = player.GetComponent<PlayerBombController>();
            bombController.RemoveBombAmount();
        }
    }
}