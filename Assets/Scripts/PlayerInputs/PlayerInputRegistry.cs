using System.Collections.Generic;
using System.Linq;
using Lobby;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInputs
{
    public class PlayerInputRegistry : MonoBehaviour
    {
        public static PlayerInputRegistry Instance;
        
        private Dictionary<int, PlayerInput> playerInputs = new Dictionary<int, PlayerInput>();
        private Dictionary<int, PlayerState> playerStates = new Dictionary<int, PlayerState>();

        private void Awake()
        {
#if UNITY_EDITOR
            if (Instance != null)
            {
                Debug.LogError("There is more than one PlayerInputRegistry in the scene!");
            }
#endif
            Instance = this;
        }

        public PlayerInput[] GetPlayerInputs()
        {
            return playerInputs.Values.ToArray();
        }
        
        public PlayerState[] GetPlayerStates()
        {
            return playerStates.Values.ToArray();
        }
        
        public int GetPlayerCount()
        {
            return playerInputs.Count;
        }
        
        public void Register(PlayerState playerState)
        {
            playerInputs.Add(playerState.PlayerIndex, playerState.PlayerInput);
            playerStates.Add(playerState.PlayerIndex, playerState);
        }
        
        public void RegisterAll(IEnumerable<PlayerState> newPlayerStates)
        {
            foreach (PlayerState playerState in newPlayerStates)
            {
                Register(playerState);
            }
        }
        
        public void Remove(int playerIndex)
        {
            playerInputs.Remove(playerIndex);
            playerStates.Remove(playerIndex);
        }
    }
}