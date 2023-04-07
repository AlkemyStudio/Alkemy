using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private LobbyController lobbyController;
        [SerializeField] private GameObject startGameText;
        [SerializeField] private LobbyPlayerSlotUI[] playerSlots;
        

        private Dictionary<int, LobbyPlayerSlotUI> _bindSlots;
        private bool _shouldUpdateUI = true;
        
        public void NoMoreUpdateUI()
        {
            _shouldUpdateUI = false;
        }

        private void Start()
        {
            _bindSlots = new Dictionary<int, LobbyPlayerSlotUI>();
            DisableAllSlots();
        }
        
        private void DisableAllSlots()
        {
            if (!_shouldUpdateUI) return;
            startGameText.SetActive(false);
            foreach (LobbyPlayerSlotUI playerSlot in playerSlots)
            {
                playerSlot.DisableSlotUI();
            }
        }
        
        public void ShowStartGameText()
        {
            if (!_shouldUpdateUI) return;
            startGameText.SetActive(true);
        }
        
        public void HideStartGameText()
        {
            if (!_shouldUpdateUI) return;
            startGameText.SetActive(false);
        }
        
        public void EnableSlotUI(int playerIndex)
        {
            if (!_shouldUpdateUI) return;
            LobbyPlayerSlotUI slot = GetFirstSlotDisable();
            slot.OnPreviousButtonClicked += lobbyController.OnSelectLeftCharacterHandler;
            slot.OnNextButtonClicked += lobbyController.OnSelectRightCharacterHandler;
            slot.OnRemoveButtonClicked += lobbyController.OnPlayerRemoveHandler;
            slot.EnableSlotUI(playerIndex);
            _bindSlots.Add(playerIndex, slot);
        }
        
        public void DisableSlotUI(int playerIndex)
        {
            if (!_shouldUpdateUI) return;
            LobbyPlayerSlotUI slot = _bindSlots[playerIndex];
            slot.DisableSlotUI();
            slot.OnPreviousButtonClicked -= lobbyController.OnSelectLeftCharacterHandler;
            slot.OnNextButtonClicked -= lobbyController.OnSelectRightCharacterHandler;
            slot.OnRemoveButtonClicked -= lobbyController.OnPlayerRemoveHandler;
            _bindSlots.Remove(playerIndex);
        }
        
        public void UpdateSlotUI(PlayerState playerState)
        {
            if (!_shouldUpdateUI) return;
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