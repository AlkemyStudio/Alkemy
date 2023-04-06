using System;
using System.Collections.Generic;
using Lobby;
using Player;
using UnityEngine;

namespace Utils
{
    public class PlayerConfigurationDebugUtils
    {
        public static void PrintPlayerConfigurations(List<PlayerState> playerConfigurations)
        {
            string result = String.Empty;
            foreach (PlayerState playerConfiguration in playerConfigurations)
            {
                result += FormatPlayerConfiguration(playerConfiguration);
            }
            
            Debug.Log(result);
        }
        
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