using UnityEngine;

namespace Player
{
    /// <summary>
    /// PlayerGameObjectRegistry is used to get the player prefab.
    /// </summary>
    public class PlayerGameObjectRegistry : MonoBehaviour
    {
        [SerializeField] private GameObject[] playerPrefabs;
        
        /// <summary>
        /// Get the player prefab with the given index.
        /// </summary>
        /// <param name="index"> The index of the player prefab </param>
        /// <returns> The player prefab </returns>
        public GameObject GetOneWithIndex(int index)
        {
            return playerPrefabs[index];
        }
    }
}