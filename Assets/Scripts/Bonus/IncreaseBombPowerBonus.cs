using Player;
using UnityEngine;

namespace Bonus
{
    public class IncreaseBombPowerBonus : BaseBonus
    {
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            var bombController = player.GetComponent<PlayerBombController>();
            bombController.AddBombPower();
        }
    }
}