using System.Collections.Generic;
using System.Linq;
using PlayerInputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Lobby
{
    [RequireComponent(
        typeof(LobbyUI), 
        typeof(LobbyCharacterRegistry)
    )]
    public class LobbyController : MonoBehaviour, IPlayerHandler
    {
        public static LobbyController Instance { get; private set; }
        
        [FormerlySerializedAs("playerInputManager")]
        [Header("References")]
        [SerializeField] private GamePlayerInputManager gamePlayerInputManager;
        [SerializeField] private LobbyUI lobbyUI;
        [SerializeField] private LobbyCharacterRegistry lobbyCharacterRegistry;
        
        [Header("Lobby Settings")]
        [SerializeField] private int minPlayerCountToStart = 2;
        [SerializeField] private int sceneBuildIndexToLoad = 2;
        
        [Header("Input Settings")]
        [SerializeField] private float delayBetweenCharacterSwitch = 0.5f;

        private Dictionary<int, PlayerState> LobbyPlayerStates { get; set; }

        private int _firstPlayerIndex = -1;
        private float _lastCharacterSwitchTime;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        private void Start()
        {
            LobbyPlayerStates = new Dictionary<int, PlayerState>();
            gamePlayerInputManager.SetPlayerHandler(this);
        }

        // This function will be called when a player joins the lobby.
        // If the player has already joined the lobby, it will update the UI.
        // If it is a new player, it will set the first selectable character, create a PlayerState, and update the UI.

        public void HandlePlayerJoined(PlayerInput playerInput)
        {
            if (LobbyPlayerStates.ContainsKey(playerInput.playerIndex))
            {
                SetIsConnected(playerInput.playerIndex, true);
                lobbyUI.UpdateSlotUI(LobbyPlayerStates[playerInput.playerIndex]);
                return;
            }
            
            int characterIndex = lobbyCharacterRegistry.GetFirstSelectableCharacterIndex();
            lobbyCharacterRegistry.SetSelectable(characterIndex, false);
            
            PlayerState playerState = new()
            {
                CharacterIndex = characterIndex,
                IsReady = false,
                IsConnected = true,
                PlayerInput = playerInput
            };
            
            if (_firstPlayerIndex == -1)
                _firstPlayerIndex = playerInput.playerIndex;
            
            LobbyPlayerStates.Add(playerInput.playerIndex, playerState);
            lobbyUI.EnableSlotUI(playerInput.playerIndex);
            lobbyUI.UpdateSlotUI(playerState);
            CreateInputBindings(playerInput);
        }

        //This code is used to handle the player input
        //The player input is handled by the PlayerInputHandler class
        //The PlayerInputHandler class is attached to the PlayerInput object

        private void CreateInputBindings(PlayerInput playerInput)
        {
            PlayerInputHandler playerInputHandler = playerInput.GetComponent<PlayerInputHandler>();
            playerInputHandler.OnDirection += OnDirectionHandler;
            playerInputHandler.OnConfirm += OnConfirmHandler;
            playerInputHandler.OnCancel += OnCancelHandler;
        }
        
       // This function is called whenever a player leaves the lobby.
       // It updates the LobbyPlayerStates dictionary and then calls the UpdateSlotUI function in the LobbyUI class.

        public void HandlePlayerLeft(PlayerInput playerInput)
        {
            if (!LobbyPlayerStates.ContainsKey(playerInput.playerIndex)) return;
            SetIsConnected(playerInput.playerIndex, false);
            SetPlayerReady(playerInput.playerIndex, false);
            lobbyUI.UpdateSlotUI(LobbyPlayerStates[playerInput.playerIndex]);
        }

        public void OnPlayerRemoveHandler(int playerIndex)
        {
            // Remove the player from the lobby
            if (!LobbyPlayerStates.ContainsKey(playerIndex)) return;
            lobbyCharacterRegistry.SetSelectable(LobbyPlayerStates[playerIndex].CharacterIndex, false);
            LobbyPlayerStates.Remove(playerIndex);

            // If the removed player was the first player, find out who is the first player now
            if (_firstPlayerIndex == playerIndex)
            {
                if (gamePlayerInputManager.GetPlayerCount() > 0)
                    _firstPlayerIndex = GetFirstConnectedPlayerIndex();
                else
                    _firstPlayerIndex = -1;
            }

            // Disable the player slot UI
            lobbyUI.DisableSlotUI(playerIndex);

            // Remove the player from the game
            gamePlayerInputManager.RemovePlayer(playerIndex);
        }


        // Gets the player index of the first connected player in the lobby.
        // Returns -1 if no players are connected.

        private int GetFirstConnectedPlayerIndex()
        {
            foreach (PlayerState playerState in LobbyPlayerStates.Values)
            {
                if (playerState.IsConnected)
                {
                    return playerState.PlayerIndex;
                }
            }
            
            return -1;
        }
        
        private void OnConfirmHandler(int playerIndex)
        {
            // If the player isn't in the lobby, return
            if (!LobbyPlayerStates.ContainsKey(playerIndex)) return;
            
            if (LobbyPlayerStates[playerIndex].IsReady == false)
            {
                // Set the player's ready state to true
                SetPlayerReady(playerIndex, true);

                // If all players are ready, show the start game text
                if (IsAllPlayersReady())
                {
                    lobbyUI.ShowStartGameText();
                }

                // Update the slot UI
                lobbyUI.UpdateSlotUI(LobbyPlayerStates[playerIndex]);
                return;
            }

            // If the player is already ready and they aren't the first player, return
            if (_firstPlayerIndex != playerIndex || !IsAllPlayersReady()) return;

            // Start the game
            StartGame();
        }

        // This function will load the scene that the player has selected in the lobby.
        // It will also pass the list of players that have been selected to the scene
        // so that the scene knows which players are in the game.

        private void StartGame()
        {
            gamePlayerInputManager.UnsetPlayerHandler();
            DisableAllPlayerInputControls();
            PlayerInputRegistry.Instance.RegisterAll(LobbyPlayerStates.Values.ToArray());
            SceneManager.LoadScene(sceneBuildIndexToLoad);
        }

        // <summary>
        // Disable all the player input controls for the players in the lobby.
        // </summary>
        private void DisableAllPlayerInputControls()
        {
            foreach (PlayerState playerState in LobbyPlayerStates.Values)
            {
                PlayerInputHandler playerInputHandler = playerState.PlayerInput.GetComponent<PlayerInputHandler>();
                playerInputHandler.OnDirection -= OnDirectionHandler;
                playerInputHandler.OnConfirm -= OnConfirmHandler;
                playerInputHandler.OnCancel -= OnCancelHandler;

                playerInputHandler.DisableInput();
            }
        }

        
        public void ReturnToMainMenu()
        {
            // Destroy the gamePlayerInputManager object
            Destroy(gamePlayerInputManager.gameObject);
            // Load the main menu scene
            SceneManager.LoadScene(0);
        }
        
        public void OnSelectLeftCharacterHandler(int playerIndex)
        {
            OnDirectionHandler(Vector2.left, playerIndex);
        }
        
        public void OnSelectRightCharacterHandler(int playerIndex)
        {
            OnDirectionHandler(Vector2.right, playerIndex);
        }

        // This function is called when a player moves the character selection wheel on the lobby menu.
        // It moves the character selection wheel to the left or right, depending on the direction
        // the player moved the wheel.
        // It also updates the player's character index.
        private void OnDirectionHandler(Vector2 direction, int playerIndex)
        {
            if (!LobbyPlayerStates.ContainsKey(playerIndex)) return;
            
            PlayerState playerState = LobbyPlayerStates[playerIndex];
            
            if (playerState.IsReady) return;
            
            int currentCharacterIndex = playerState.CharacterIndex;

            if (_lastCharacterSwitchTime + delayBetweenCharacterSwitch > Time.time) return;
            _lastCharacterSwitchTime = Time.time;
            
            int nextCharacterIndex = direction.x > 0
                ? lobbyCharacterRegistry.GetNextSelectableCharacterIndex(currentCharacterIndex)
                : lobbyCharacterRegistry.GetPreviousSelectableCharacterIndex(currentCharacterIndex);
            
            lobbyCharacterRegistry.SetSelectable(currentCharacterIndex, true);
            lobbyCharacterRegistry.SetSelectable(nextCharacterIndex, false);
            
            SetPlayerCharacterIndex(playerIndex, nextCharacterIndex);
            lobbyUI.UpdateSlotUI(LobbyPlayerStates[playerIndex]);
        }



        // Checks if all players are ready and if the number of players is greater than the minimum required to start the game.
        // Returns true if all players are ready and the minimum number of players is reached, false otherwise.

        private bool IsAllPlayersReady()
        {
            return 
                LobbyPlayerStates.All(playerState => playerState.Value.IsReady) &&
                LobbyPlayerStates.Count >= minPlayerCountToStart;
        }
        
        /// <summary>
        /// This function is called when a player presses the cancel button on the lobby menu.
        /// If the player is ready, it will set the player's ready state to false.
        /// If the player is not ready, it will return the player to the main menu.
        /// </summary>
        private void OnCancelHandler(int playerIndex)
        {
            if (!LobbyPlayerStates.ContainsKey(playerIndex)) 
                return;
            
            if (LobbyPlayerStates[playerIndex].IsReady == false)
                ReturnToMainMenu();
            
            SetPlayerReady(playerIndex, false);
            lobbyUI.UpdateSlotUI(LobbyPlayerStates[playerIndex]);
            lobbyUI.HideStartGameText();
        }
        
        /// <summary>
        /// This function is called when a player presses the start button on the lobby menu.
        /// If the player is the first player in the lobby, it will start the game.
        /// </summary>
        private void SetIsConnected(int playerIndex, bool state)
        {
            PlayerState playerState = LobbyPlayerStates[playerIndex];
            playerState.IsConnected = state;
            LobbyPlayerStates[playerIndex] = playerState;
        }
        
        /// <summary>
        /// This function is called when a player presses the start button on the lobby menu.
        /// If the player is the first player in the lobby, it will start the game.
        /// </summary>
        private void SetPlayerReady(int playerIndex, bool state)
        {
            PlayerState playerState = LobbyPlayerStates[playerIndex];
            playerState.IsReady = state;
            LobbyPlayerStates[playerIndex] = playerState;
        }

        /// <summary>
        /// This function is called when a player presses the start button on the lobby menu.
        /// If the player is the first player in the lobby, it will start the game.
        /// </summary>
        private void SetPlayerCharacterIndex(int playerIndex, int characterIndex)
        {
            PlayerState playerState = LobbyPlayerStates[playerIndex];
            playerState.CharacterIndex = characterIndex;
            LobbyPlayerStates[playerIndex] = playerState;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (lobbyUI == null)
                lobbyUI = GetComponent<LobbyUI>();
            
            if (lobbyCharacterRegistry == null)
                lobbyCharacterRegistry = GetComponent<LobbyCharacterRegistry>();
        }
#endif
    }
}