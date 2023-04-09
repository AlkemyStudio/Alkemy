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

        // This function returns the index of the first character that is selectable.
        // This is used when the player chooses a character and goes back to the character selection
        // screen. At that point we want to select the character that they had previously selected.
        // If they don't have a selectable character, we select the first selectable character.

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
        
        // Returns the index of the next selectable character, starting from the given index
        // Returns the given index if no other selectable character is found
        // Returns -1 if there are no selectable characters
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
            //This function is called when a player wants to select a character
            //It goes through the lobbyCharacters array and returns the index of the previous character that is selectable on the array
            //The for loop starts from the end of the array and goes backwards
            //It uses the mod operator to get the index of the previous character
            //It checks if the character is selectable and returns its index if it is
            //If there isn't a selectable character, it returns the current index
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
            LobbyCharacter lobbyCharacter = lobbyCharacters[index];
            lobbyCharacter.isSelectable = isSelectable;
            lobbyCharacters[index] = lobbyCharacter;
        }
    }
}