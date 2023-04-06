using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lobby
{
    [RequireComponent(typeof(LobbyController))]
    public class LobbyPlayerManager : MonoBehaviour
    {
        [SerializeField] private LobbyController lobbyController;
        private List<PlayerInput> _playerInputs;

        private void Start()
        {
            _playerInputs = new List<PlayerInput>();
        }

        public void HandlePlayerJoined(PlayerInput playerInput)
        {
            Debug.Log("A player joined!");
            
            // Set the parent of the referenced PlayerInput's GameObject to this object.
            playerInput.transform.SetParent(transform);
            _playerInputs.Add(playerInput);
            
            lobbyController.PlayerJoinedHandler(playerInput);
        }
        
        public void HandlePlayerLeft(PlayerInput playerInput)
        {
            Debug.Log("A player left!");
            
            _playerInputs.Remove(playerInput);
            lobbyController.PlayerDisconnectHandler(playerInput);
        }
        
        public int GetPlayerCount()
        {
            return _playerInputs.Count;
        }
        
        public int GetFirstPlayerIndex()
        {
            return _playerInputs[0].playerIndex;
        }
        
        public void RemovePlayer(int playerIndex)
        {
            PlayerInput playerInput = _playerInputs.FirstOrDefault(p => p.playerIndex == playerIndex);
            if (playerInput == null)
                return;
            
            _playerInputs.Remove(playerInput);
            Destroy(playerInput.gameObject);
        }
    }
}