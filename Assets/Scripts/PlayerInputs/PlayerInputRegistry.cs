using System.Collections.Generic;
using System.Linq;
using Lobby;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInputs
{
    /// <summary>
    /// PlayerInputRegistry is used to keep track of all the PlayerInputs in the scene.
    /// </summary>
    public class PlayerInputRegistry : MonoBehaviour
    {
        public static PlayerInputRegistry Instance;
        
        private Dictionary<int, PlayerInput> playerInputs = new Dictionary<int, PlayerInput>();
        private Dictionary<int, PlayerState> playerStates = new Dictionary<int, PlayerState>();

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
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

        /// <summary>
        /// This method is used to get the PlayerInput of a player.
        /// </summary>
        /// <returns> The PlayerInput of the player </returns>
        public PlayerInput[] GetPlayerInputs()
        {
            return playerInputs.Values.ToArray();
        }
        
        /// <summary>
        /// This method is used to get the PlayerState of a player.
        /// </summary>
        /// <returns> The PlayerState of the player </returns>
        public PlayerState[] GetPlayerStates()
        {
            return playerStates.Values.ToArray();
        }
        
        /// <summary>
        /// This method is used to get the number of players.
        /// </summary>
        /// <returns></returns>
        public int GetPlayerCount()
        {
            return playerInputs.Count;
        }
        
        /// <summary>
        /// Register a new player state.
        /// </summary>
        /// <param name="playerState"> The player state to register </param>
        public void Register(PlayerState playerState)
        {
            playerInputs.Add(playerState.PlayerIndex, playerState.PlayerInput);
            playerStates.Add(playerState.PlayerIndex, playerState);
        }
        
        /// <summary>
        /// Register all player states contained in the given list.
        /// </summary>
        /// <param name="newPlayerStates"> The list of player states to register </param>
        public void RegisterAll(IEnumerable<PlayerState> newPlayerStates)
        {
            foreach (PlayerState playerState in newPlayerStates)
            {
                Register(playerState);
            }
        }
        
        /// <summary>
        /// Remove a player state.
        /// </summary>
        /// <param name="playerIndex"> The player index of the player state to remove </param>
        public void Remove(int playerIndex)
        {
            playerInputs.Remove(playerIndex);
            playerStates.Remove(playerIndex);
        }
    }
}