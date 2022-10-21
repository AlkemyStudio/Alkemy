
using System;
using UnityEngine;

namespace Character
{
    [Serializable]
    public struct CharacterState
    {
        
        [Header("Character base stats")]
        public int speed;

        [Header("Bomb base stats")] 
        public GameObject bombPrefabs;
        public int bombPower;
        public int simultaneousBomb;
    }
} 
