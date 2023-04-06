using Lobby;
using UnityEngine;

namespace Game
{
    public class SetupCharacterRegistry : MonoBehaviour
    {
        [SerializeField] private GameObject[] characterPrefabs; 
        
        private void Awake()
        {
            SelectableCharacterRegistry selectableCharacterRegistry = SelectableCharacterRegistry.Instance;
            foreach (GameObject characterPrefab in characterPrefabs)
            {
                selectableCharacterRegistry.Register(characterPrefab);
            }
        }
    }
}