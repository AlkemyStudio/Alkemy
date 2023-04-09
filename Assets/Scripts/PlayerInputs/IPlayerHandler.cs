using UnityEngine.InputSystem;

namespace PlayerInputs
{
    /// <summary>
    /// IPlayerHandler is used to handle the input of a player.
    /// </summary>
    public interface IPlayerHandler
    {
        public void HandlePlayerJoined(PlayerInput playerInput);
        public void HandlePlayerLeft(PlayerInput playerInput);
    }
}