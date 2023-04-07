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

        private void CreateInputBindings(PlayerInput playerInput)
        {
            PlayerInputHandler playerInputHandler = playerInput.GetComponent<PlayerInputHandler>();
            playerInputHandler.OnDirection += OnDirectionHandler;
            playerInputHandler.OnConfirm += OnConfirmHandler;
            playerInputHandler.OnCancel += OnCancelHandler;
        }
        
        public void HandlePlayerLeft(PlayerInput playerInput)
        {
            if (!LobbyPlayerStates.ContainsKey(playerInput.playerIndex)) return;
            SetIsConnected(playerInput.playerIndex, false);
            SetPlayerReady(playerInput.playerIndex, false);
            lobbyUI.UpdateSlotUI(LobbyPlayerStates[playerInput.playerIndex]);
        }

        public void OnPlayerRemoveHandler(int playerIndex)
        {
            if (!LobbyPlayerStates.ContainsKey(playerIndex)) return;
            lobbyCharacterRegistry.SetSelectable(LobbyPlayerStates[playerIndex].CharacterIndex, false);
            LobbyPlayerStates.Remove(playerIndex);

            if (_firstPlayerIndex == playerIndex)
            {
                if (gamePlayerInputManager.GetPlayerCount() > 0)
                    _firstPlayerIndex = GetFirstConnectedPlayerIndex();
                else
                    _firstPlayerIndex = -1;
            }

            lobbyUI.DisableSlotUI(playerIndex);
            gamePlayerInputManager.RemovePlayer(playerIndex);
        }

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
            if (!LobbyPlayerStates.ContainsKey(playerIndex)) return;
            
            if (LobbyPlayerStates[playerIndex].IsReady == false)
            {
                SetPlayerReady(playerIndex, true);

                if (IsAllPlayersReady())
                {
                    lobbyUI.ShowStartGameText();
                }

                lobbyUI.UpdateSlotUI(LobbyPlayerStates[playerIndex]);
                return;
            }

            if (_firstPlayerIndex != playerIndex || !IsAllPlayersReady()) return;

            StartGame();
        }

        private void StartGame()
        {
            gamePlayerInputManager.UnsetPlayerHandler();
            DisableAllPlayerInputControls();
            PlayerInputRegistry.Instance.RegisterAll(LobbyPlayerStates.Values.ToArray());
            SceneManager.LoadScene(sceneBuildIndexToLoad);
        }

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
            Destroy(gameObject);
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

        private bool IsAllPlayersReady()
        {
            return 
                LobbyPlayerStates.All(playerState => playerState.Value.IsReady) &&
                LobbyPlayerStates.Count >= minPlayerCountToStart;
        }
        
        private void OnCancelHandler(int playerIndex)
        {
            if (!LobbyPlayerStates.ContainsKey(playerIndex)) return;
            SetPlayerReady(playerIndex, false);
            lobbyUI.UpdateSlotUI(LobbyPlayerStates[playerIndex]);
            lobbyUI.HideStartGameText();
        }
        
        private void SetIsConnected(int playerIndex, bool state)
        {
            PlayerState playerState = LobbyPlayerStates[playerIndex];
            playerState.IsConnected = state;
            LobbyPlayerStates[playerIndex] = playerState;
        }
        
        private void SetPlayerReady(int playerIndex, bool state)
        {
            PlayerState playerState = LobbyPlayerStates[playerIndex];
            playerState.IsReady = state;
            LobbyPlayerStates[playerIndex] = playerState;
        }

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