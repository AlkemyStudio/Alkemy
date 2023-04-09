using System;
using System.Collections.Generic;
using Lobby;
using Player;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// This class is used to print player configurations
    /// </summary>
    public class PlayerConfigurationDebugUtils
    {
        /// <summary>
        /// This method is used to print player configurations
        /// </summary>
        /// <param name="playerConfigurations"> The player configurations to print </param>
        public static void PrintPlayerConfigurations(List<PlayerState> playerConfigurations)
        {
            string result = String.Empty;
            foreach (PlayerState playerConfiguration in playerConfigurations)
            {
                result += FormatPlayerConfiguration(playerConfiguration);
            }
            
            Debug.Log(result);
        }
        
        /// <summary>
        /// This method is used to format a player configuration
        /// </summary>
        /// <param name="playerState"> The player configuration to format </param>
        /// <returns> The formatted player configuration </returns>
        public static string FormatPlayerConfiguration(PlayerState playerState)
        {
            string result = "PlayerConfiguration (player index = " + playerState.PlayerInput.playerIndex + "): {\n";
            result += "  Input: {\n";
            result += "    playerIndex: " + playerState.PlayerInput.playerIndex + "\n";
            result += "  }\n";
            result += "  Player: {\n";
            result += "    playerIndex: " + playerState.PlayerIndex + "\n";
            result += "    characterIndex: " + playerState.CharacterIndex + "\n";
            result += "  }\n";
            result += "}\n";

            return result;
        }
    }
}