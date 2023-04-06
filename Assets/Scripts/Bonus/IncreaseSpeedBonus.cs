using Player;
using UnityEngine;

namespace Bonus
{
    public class IncreaseSpeedBonus : BaseBonus
    {
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            player.GetComponent<PlayerMovement>().AddSpeedBonus();
        }
    }
}