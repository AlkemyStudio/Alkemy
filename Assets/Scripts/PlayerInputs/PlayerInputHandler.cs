using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

namespace PlayerInputs
{
    /// <summary>
    /// PlayerInputHandler is used to handle the input of a player.
    /// </summary>
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

        /// <summary>
        /// Start is called when the script instance is being loaded.
        /// </summary>
        private void Start()
        {
            _controls = new GameInputController();
            
            if (SceneManager.GetActiveScene().name == "Lobby")
                UseMenuControls();
            else
                UsePlayerControls();
            
            playerInput.onActionTriggered += OnActionTriggered;
        }
        
        /// <summary>
        /// Use the player controls.
        /// </summary>
        public void UsePlayerControls()
        {
            _controls.Player.Enable();
            _controls.Menu.Disable();
            playerInput.SwitchCurrentActionMap("Player");
        }
        
        /// <summary>
        /// Use the menu controls.
        /// </summary>
        public void UseMenuControls()
        {
            _controls.Menu.Enable();
            _controls.Player.Disable();
            playerInput.SwitchCurrentActionMap("Menu");
        }

        /// <summary>
        /// Enable the input.
        /// </summary>
        public void EnableInput()
        {
            playerInput.ActivateInput();
        }
        
        /// <summary>
        /// Disable the input.
        /// </summary>
        public void DisableInput()
        {
            playerInput.DeactivateInput();
        }

        /// <summary>
        /// OnActionTriggered is called when an input action is triggered.
        /// </summary>
        /// <param name="context"> The context of the input action </param>
        private void OnActionTriggered(CallbackContext context)
        {
            if (playerInput.devices[0].deviceId != context.control.device.deviceId) return;

            string actionName = context.action.name;

            if (actionName == _controls.Menu.Direction.name) {
                OnDirection?.Invoke(context.ReadValue<Vector2>(), playerInput.playerIndex);
            } else if (actionName == _controls.Menu.Confirm.name) {
                if (context.performed) 
                    OnConfirm?.Invoke(playerInput.playerIndex);
            } else if (actionName == _controls.Menu.Cancel.name) {
                if (context.performed) 
                    OnCancel?.Invoke(playerInput.playerIndex);
            } else if (actionName == _controls.Player.Move.name) {
                OnMove?.Invoke(context.ReadValue<Vector2>());
            } else if (actionName == _controls.Player.PlaceBomb.name) {
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