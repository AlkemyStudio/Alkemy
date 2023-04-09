using System.Collections;
using Player;
using UnityEngine;

namespace Bonus
{
    public class PushBombBonus : BaseBonus
    {
        /// <summary>
        /// Increases the bomb amount of the player
        /// </summary>
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            player.GetComponent<PlayerEffects>().SetPushBombEffect(true);
        }
    }
}