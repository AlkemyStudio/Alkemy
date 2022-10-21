using System;
using UnityEngine;

namespace Bomb
{
    [Serializable]
    public struct BombData
    {
        public const float CountdownDuration = 3f;
        public const float ExplosionDuration = 0.3f;
        
        public int power;
        
        public BombData(int power)
        {
            this.power = power;
        }
    }
}