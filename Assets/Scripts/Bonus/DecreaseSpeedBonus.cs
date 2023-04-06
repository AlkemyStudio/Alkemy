using Player;
using UnityEngine;

namespace Bonus
{
    public class DecreaseSpeedBonus : BaseBonus
    {
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            player.GetComponent<PlayerMovement>().RemoveSpeedBonus();
        }
    }
}