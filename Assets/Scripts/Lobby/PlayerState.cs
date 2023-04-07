using UnityEngine.InputSystem;

namespace Lobby
{
    public struct PlayerState
    {
        public int PlayerIndex => PlayerInput.playerIndex;
        public PlayerInput PlayerInput;
        public int CharacterIndex;
        public bool IsReady;
        public bool IsConnected;
    }
}