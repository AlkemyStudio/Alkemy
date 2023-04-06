using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerConfiguration
    {
        public bool IsBot { get; set; }
        
        public PlayerInput Input { get; set; }
        public int PlayerIndex => Input.playerIndex;
        
        public bool IsReady { get; set; }
        
        public int CharacterIndex { get; set; }
        
        public PlayerConfiguration(PlayerInput playerInput, int characterIndex, bool isBot = false)
        {
            Input = playerInput;
            CharacterIndex = characterIndex;
            IsBot = isBot;
        }

        public PlayerConfiguration(int characterIndex, bool isBot = false)
        {
            Input = null;
            CharacterIndex = characterIndex;
            IsBot = isBot;
        }
    }
}