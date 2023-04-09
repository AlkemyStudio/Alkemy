using System.Collections;
using Player;
using UnityEngine;

namespace Bonus
{
    public class PushBombBonus : BaseBonus
    {
        protected override void OnPlayerTakeBonus(GameObject player)
        {
            player.GetComponent<PlayerEffects>().SetPushBombEffect(true);
        }
    }
}