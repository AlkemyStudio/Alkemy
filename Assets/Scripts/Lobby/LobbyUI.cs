using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private LobbyController lobbyController;
        [SerializeField] private GameObject startGameText;
        [SerializeField] private LobbyPlayerSlotUI[] playerSlots;
        

        private Dictionary<int, LobbyPlayerSlotUI> _bindSlots;

        private void Start()
        {
            _bindSlots = new Dictionary<int, LobbyPlayerSlotUI>();
            DisableAllSlots();
        }
        
        private void DisableAllSlots()
        {
            startGameText.SetActive(false);
            foreach (LobbyPlayerSlotUI playerSlot in playerSlots)
            {
                playerSlot.DisableSlotUI();
            }
        }
        
        public void ShowStartGameText()
        {
            startGameText.SetActive(true);
        }
        
        public void HideStartGameText()
        {
            startGameText.SetActive(false);
        }
        
        public void EnableSlotUI(int playerIndex)
        {
            LobbyPlayerSlotUI slot = GetFirstSlotDisable();
            slot.OnPreviousButtonClicked += lobbyController.OnSelectLeftCharacterHandler;
            slot.OnNextButtonClicked += lobbyController.OnSelectRightCharacterHandler;
            slot.OnRemoveButtonClicked += lobbyController.OnPlayerRemoveHandler;
            slot.EnableSlotUI(playerIndex);
            _bindSlots.Add(playerIndex, slot);
        }
        
        public void DisableSlotUI(int playerIndex)
        {
            LobbyPlayerSlotUI slot = _bindSlots[playerIndex];
            slot.DisableSlotUI();
            slot.OnPreviousButtonClicked -= lobbyController.OnSelectLeftCharacterHandler;
            slot.OnNextButtonClicked -= lobbyController.OnSelectRightCharacterHandler;
            slot.OnRemoveButtonClicked -= lobbyController.OnPlayerRemoveHandler;
            _bindSlots.Remove(playerIndex);
        }
        
        public void UpdateSlotUI(PlayerState playerState)
        {
            if (playerState.PlayerIndex == -1)
            {
                return;
            }

            _bindSlots[playerState.PlayerIndex].UpdateUI(playerState);
        }
        
        private LobbyPlayerSlotUI GetFirstSlotDisable()
        {
            return playerSlots.FirstOrDefault(playerSlot => playerSlot.IsDisable);
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (lobbyController == null)
                lobbyController = GetComponent<LobbyController>();
        }
#endif
    }
}