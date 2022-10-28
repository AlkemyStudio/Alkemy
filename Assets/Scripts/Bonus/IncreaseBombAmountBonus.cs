using Player;
using UnityEngine;

namespace Bonus
{
    public class IncreaseBombAmountBonus : BaseBonus
    {
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            var bombController = player.GetComponent<PlayerBombController>();
            bombController.AddBombAmount();
        }
    }
}