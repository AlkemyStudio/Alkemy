using System;
using UnityEngine;

namespace Lobby
{
    [Serializable]
    public struct LobbyCharacter
    {
        public GameObject prefab;
        public bool isSelectable;
    }
    
    public class LobbyCharacterRegistry : MonoBehaviour
    {
        [SerializeField] private LobbyCharacter[] lobbyCharacters;

        public int GetFirstSelectableCharacterIndex()
        {
            for (int i = 0; i < lobbyCharacters.Length; i++)
            {
                if (lobbyCharacters[i].isSelectable)
                {
                    return i;
                }
            }

            return 0;
        }
        
        public int GetNextSelectableCharacterIndex(int currentIndex)
        {
            for (int i = 0; i < lobbyCharacters.Length; i++)
            {
                int index = (i + currentIndex) % lobbyCharacters.Length;
                
                if (lobbyCharacters[index].isSelectable)
                {
                    return index;
                }
            }

            return currentIndex;
        }
        
        public int GetPreviousSelectableCharacterIndex(int currentIndex)
        {
            for (int i = lobbyCharacters.Length - 1; i >= 0; i--)
            {
                int index = (i + currentIndex) % lobbyCharacters.Length;
                
                if (lobbyCharacters[index].isSelectable)
                {
                    return index;
                }
            }

            return currentIndex;
        }
        
        public GameObject GetPrefabWithIndex(int index)
        {
            return lobbyCharacters[index].prefab;
        }
        
        public void SetSelectable(int index, bool isSelectable)
        {
            lobbyCharacters[index].isSelectable = isSelectable;
        }
    }
}