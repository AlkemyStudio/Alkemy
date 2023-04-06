using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

namespace Lobby
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        private GameInputController _controls;
        
        public event Action<Vector2, int> OnDirection;
        public event Action<int> OnConfirm;
        public event Action<int> OnCancel;
        
        public event Action<Vector2> OnMove;
        public event Action OnPlaceBomb;

        private void Start()
        {
            _controls = new GameInputController();
            
            if (SceneManager.GetActiveScene().name == "Lobby")
                UseMenuControls();
            else
                UsePlayerControls();
            
            playerInput.onActionTriggered += OnActionTriggered;
        }
        
        public void UsePlayerControls()
        {
            _controls.Player.Enable();
            _controls.Menu.Disable();
            playerInput.SwitchCurrentActionMap("Player");
        }
        
        public void UseMenuControls()
        {
            _controls.Menu.Enable();
            _controls.Player.Disable();
            playerInput.SwitchCurrentActionMap("Menu");
        }

        public void EnableInput()
        {
            playerInput.ActivateInput();
        }
        
        public void DisableInput()
        {
            playerInput.DeactivateInput();
        }

        private void OnActionTriggered(CallbackContext context)
        {
            if (playerInput.devices[0].deviceId != context.control.device.deviceId) return;

            string actionName = context.action.name;
            Debug.Log($"actionName: {actionName}");

            if (actionName == _controls.Menu.Direction.name) {
                OnDirection?.Invoke(context.ReadValue<Vector2>(), playerInput.playerIndex);
            } else if (actionName == _controls.Menu.Confirm.name) {
                if (context.performed) 
                    OnConfirm?.Invoke(playerInput.playerIndex);
            } else if (actionName == _controls.Menu.Cancel.name) {
                if (context.performed) 
                    OnCancel?.Invoke(playerInput.playerIndex);
            } else if (actionName == _controls.Player.Move.name) {
                Debug.Log("Invoke Move");
                OnMove?.Invoke(context.ReadValue<Vector2>());
            } else if (actionName == _controls.Player.PlaceBomb.name) {
                Debug.Log("Invoke Place bomb");
                if (context.performed) 
                    OnPlaceBomb?.Invoke();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();
        }
#endif
    }
}