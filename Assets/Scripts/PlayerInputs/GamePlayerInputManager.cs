using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PlayerInputs
{
    /// <summary>
    /// GamePlayerInputManager is used to handle the input of the players.
    /// </summary>
    [RequireComponent(typeof(PlayerInputManager), typeof(PlayerInputRegistry))]
    public class GamePlayerInputManager : MonoBehaviour
    {
        [SerializeField] private IPlayerHandler playerHandler;
        [SerializeField] private PlayerInputManager gamePlayerInputManager;
        [SerializeField] private PlayerInputRegistry playerInputRegistry;
     
        /// <summary>
        /// Set the player handler.
        /// </summary>
        /// <param name="newPlayerHandler"></param>
        public void SetPlayerHandler(IPlayerHandler newPlayerHandler)
        {
            playerHandler = newPlayerHandler;
        }
        
        /// <summary>
        /// Unset the player handler.
        /// </summary>
        public void UnsetPlayerHandler()
        {
            playerHandler = null;
        }

        /// <summary>
        /// Handle the player joined event.
        /// </summary>
        /// <param name="playerInput"></param>
        public void HandlePlayerJoined(PlayerInput playerInput)
        {
            GameObject playerInputGo = playerInput.gameObject;
            playerInputGo.transform.SetParent(transform);
            playerInputGo.name = $"Player Input {playerInput.playerIndex}";
            
            playerHandler?.HandlePlayerJoined(playerInput);
        }

        /// <summary>
        /// Handle the player left event.
        /// </summary>
        /// <param name="playerInput"></param>
        public void HandlePlayerLeft(PlayerInput playerInput)
        {
            playerHandler?.HandlePlayerLeft(playerInput);
        }
        
        /// <summary>
        /// Get the player count. 
        /// </summary>
        /// <returns></returns>
        public int GetPlayerCount()
        {
            return playerInputRegistry.GetPlayerCount();
        }
        
        /// <summary>
        /// Remove the player.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        public void RemovePlayer(int playerIndex)
        {
            playerInputRegistry.Remove(playerIndex);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gamePlayerInputManager == null)
            {
                gamePlayerInputManager = GetComponent<PlayerInputManager>();
            }

            if (playerInputRegistry == null)
            {
                playerInputRegistry = GetComponent<PlayerInputRegistry>();
            }
        }
#endif
    }
}