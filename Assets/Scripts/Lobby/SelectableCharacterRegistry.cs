using System.Collections.Generic;
using UnityEngine;

namespace Lobby
{
    public class SelectableCharacterRegistry
    {
        private static SelectableCharacterRegistry _instance;
        public static SelectableCharacterRegistry Instance => _instance ??= new SelectableCharacterRegistry();

        public int NumberOfSelectableCharacters => SelectableCharacters.Count;
        public List<SelectableCharacter> SelectableCharacters { get; }

        private SelectableCharacterRegistry()
        {
            SelectableCharacters = new List<SelectableCharacter>();
        }

        public SelectableCharacter GetSelectableCharacterAt(int index)
        {
            return SelectableCharacters[index];
        }
        
        public void Register(GameObject prefab)
        {
            SelectableCharacters.Add(new SelectableCharacter(prefab));
        }
        
        public void Clear()
        {
            SelectableCharacters.Clear();
        }
        
        public int GetNextSelectableCharacterIndex(int currentIndex)
        {
            for (int i = 0; i < SelectableCharacters.Count; i++)
            {
                int index = (i + currentIndex) % SelectableCharacters.Count;
                
                if (SelectableCharacters[index].isSelectable)
                {
                    return index;
                }
            }

            return currentIndex;
        }
        
        public int GetPreviousSelectableCharacterIndex(int currentIndex)
        {
            for (int i = SelectableCharacters.Count - 1; i >= 0; i--)
            {
                int index = (i + currentIndex) % SelectableCharacters.Count;
                
                if (SelectableCharacters[index].isSelectable)
                {
                    return index;
                }
            }

            return currentIndex;
        }
        
        public void SetSelectable(int index, bool isSelectable)
        {
            SelectableCharacters[index].SetSelectable(isSelectable);
        }
    }
}