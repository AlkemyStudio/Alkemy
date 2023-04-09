using System;
using UnityEngine;

namespace Bomb
{
    [Serializable]
    public struct BombData
    {
        public const float ExplosionDuration = 0.8f;
        
        public int power;
        
        public BombData(int power)
        {
            this.power = power;
        }
    }
}