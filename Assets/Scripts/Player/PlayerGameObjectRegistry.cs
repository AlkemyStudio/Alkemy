using UnityEngine;

namespace Player
{
    public class PlayerGameObjectRegistry : MonoBehaviour
    {
        [SerializeField] private GameObject[] playerPrefabs;
        
        public GameObject GetOneWithIndex(int index)
        {
            return playerPrefabs[index];
        }
    }
}