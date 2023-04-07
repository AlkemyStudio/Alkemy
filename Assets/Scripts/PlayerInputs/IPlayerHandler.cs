using UnityEngine.InputSystem;

namespace PlayerInputs
{
    public interface IPlayerHandler
    {
        public void HandlePlayerJoined(PlayerInput playerInput);
        public void HandlePlayerLeft(PlayerInput playerInput);
    }
}