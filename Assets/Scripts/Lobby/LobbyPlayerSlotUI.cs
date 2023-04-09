using System;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyPlayerSlotUI : MonoBehaviour
    {
        public event Action<int> OnNextButtonClicked;
        public event Action<int> OnPreviousButtonClicked;
        public event Action<int> OnRemoveButtonClicked;
        
        [SerializeField] private GameObject characterRenderer;
        [SerializeField] private GameObject characterGameObjectPosition;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button removeButton;
        [SerializeField] private GameObject readyMark;

        private int _playerIndex;
        private LobbyCharacterRegistry _lobbyCharacterRegistry;
        private GameObject _characterGameObject;
        private int _previousCharacterIndex = -1;

        public bool IsDisable => _playerIndex == -1;
        
        public void UpdateUI(PlayerState playerState)
        {
            try {
                if (readyMark != null) {
                    readyMark.SetActive(playerState.IsReady);
                }

                if (_previousCharacterIndex != playerState.CharacterIndex) {
                    UpdateCharacterRenderer(playerState.CharacterIndex);
                }
            } catch(Exception e) {
            }
            // TODO: Update player connection state
        }

        private void UpdateCharacterRenderer(int characterIndex)
        {
            if (_characterGameObject != null)
            {
                Destroy(_characterGameObject);
            }
            
            _previousCharacterIndex = characterIndex;
            GameObject prefab = _lobbyCharacterRegistry.GetPrefabWithIndex(characterIndex);
            GameObject newGameObject = Instantiate(prefab, characterGameObjectPosition.transform);
            _characterGameObject = newGameObject;
        }

        public void EnableSlotUI(int playerIndex)
        {
            _lobbyCharacterRegistry = LobbyController.Instance.GetComponent<LobbyCharacterRegistry>();
            _playerIndex = playerIndex;
            EnableCharacterRenderer();
            EnableNextButton();
            EnablePreviousButton();
            EnableRemoveButton();
        }
        
        public void DisableSlotUI()
        {
            _playerIndex = -1;
            DisableCharacterRenderer();
            DisableNextButton();
            DisablePreviousButton();
            DisableRemoveButton();
            DisableMarkReadyButton();
        }
        
        private void EnableCharacterRenderer()
        {
            characterRenderer.SetActive(true);
        }
        
        private void DisableCharacterRenderer()
        {
            characterRenderer.SetActive(false);
        }
        
        private void EnableNextButton()
        {
            nextButton.gameObject.SetActive(true);
            nextButton.onClick.AddListener(HandleNextButtonClicked);
        }
        
        private void DisableNextButton()
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.RemoveListener(HandleNextButtonClicked);
        }
        
        private void HandleNextButtonClicked()
        {
            OnNextButtonClicked?.Invoke(_playerIndex);
        }
        
        private void EnablePreviousButton()
        {
            previousButton.gameObject.SetActive(true);
            previousButton.onClick.AddListener(HandlePreviousButtonClicked);
        }
        
        private void DisablePreviousButton()
        {
            previousButton.gameObject.SetActive(false);
            previousButton.onClick.RemoveListener(HandlePreviousButtonClicked);
        }

        private void HandlePreviousButtonClicked()
        {
            OnPreviousButtonClicked?.Invoke(_playerIndex);
        }
        
        private void EnableRemoveButton()
        {
            removeButton.gameObject.SetActive(true);
            removeButton.onClick.AddListener(HandleRemoveButtonClicked);
        }
        
        private void DisableRemoveButton()
        {
            removeButton.gameObject.SetActive(false);
            removeButton.onClick.RemoveListener(HandleRemoveButtonClicked);
        }
        
        private void HandleRemoveButtonClicked()
        {
            OnRemoveButtonClicked?.Invoke(_playerIndex);
        }
        
        private void DisableMarkReadyButton()
        {
            readyMark.SetActive(false);
        }
    }
}