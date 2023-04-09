using Player;
using UnityEngine;

namespace Bonus
{
    public class IncreaseBombAmountBonus : BaseBonus
    {
        /// <summary>
        /// Increases the bomb amount of the player
        /// </summary>
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            var bombController = player.GetComponent<PlayerBombController>();
            bombController.AddBombAmount();
        }
    }
}