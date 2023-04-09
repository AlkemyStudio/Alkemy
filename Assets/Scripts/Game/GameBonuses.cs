using System;
using System.Linq;
using Bonus;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Game
{
    public class GameBonuses : MonoBehaviour
    {
        public static GameBonuses Instance;

        [SerializeField] private GameBonus[] modifiersTable;

        private int _totalProbability;
        private int[] _cumulativeProbabilities;

        private void Awake()
        {
#if UNITY_EDITOR
            if (Instance != null) { 
                Debug.LogError("Multiple GameBonuses instances detected!");
            }
#endif
            Instance = this;
        }

        private void Start()
        {
            _totalProbability = 0;
            _cumulativeProbabilities = new int[modifiersTable.Length];
            
            for (int i = 0; i < modifiersTable.Length; i++)
            {
                _totalProbability += modifiersTable[i].probability;
                _cumulativeProbabilities[i] = _totalProbability;
            }
        }
        
        public BaseBonus GetRandomBonus()
        {
            int randomValue = Random.Range(0, _totalProbability);
            for (int i = 0; i < _cumulativeProbabilities.Length; i++)
            {
                if (randomValue <= _cumulativeProbabilities[i])
                {
                    return modifiersTable[i].prefab;
                }
            }

            return null;
        }
    }
}
