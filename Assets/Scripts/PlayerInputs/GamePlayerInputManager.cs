using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PlayerInputs
{
    [RequireComponent(typeof(PlayerInputManager), typeof(PlayerInputRegistry))]
    public class GamePlayerInputManager : MonoBehaviour
    {
        [SerializeField] private IPlayerHandler playerHandler;
        [SerializeField] private PlayerInputManager gamePlayerInputManager;
        [SerializeField] private PlayerInputRegistry playerInputRegistry;
        
        public void SetPlayerHandler(IPlayerHandler newPlayerHandler)
        {
            playerHandler = newPlayerHandler;
        }
        
        public void UnsetPlayerHandler()
        {
            playerHandler = null;
        }

        public void HandlePlayerJoined(PlayerInput playerInput)
        {
            GameObject playerInputGo = playerInput.gameObject;
            playerInputGo.transform.SetParent(transform);
            playerInputGo.name = $"Player Input {playerInput.playerIndex}";
            
            playerHandler?.HandlePlayerJoined(playerInput);
        }

        public void HandlePlayerLeft(PlayerInput playerInput)
        {
            playerHandler?.HandlePlayerLeft(playerInput);
        }

        public int GetPlayerCount()
        {
            return playerInputRegistry.GetPlayerCount();
        }
        
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