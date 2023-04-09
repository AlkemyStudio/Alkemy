using System;
using Bonus;
using UnityEngine;

namespace Game
{
    [Serializable]
    public struct GameBonus
    {
        public BaseBonus prefab;
        
        [Min(0)]
        public int probability;
    }
}