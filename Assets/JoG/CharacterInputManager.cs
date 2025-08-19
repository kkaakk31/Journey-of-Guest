using ANU.IngameDebug.Console;
using EditorAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils;

namespace JoG {

    [DebugCommandPrefix("input")]
    public class CharacterInputManager : Singleton<CharacterInputManager> {
        [SerializeField, ReadOnly] protected int _enableInputCount = 0;
        private InputActionMap _inputActions;
        public InputActionMap InputActions => _inputActions;

        [DebugCommand]
        public void EnableInput() {
            _enableInputCount++;
            UpdateInputState();
        }

        [DebugCommand]
        public void DisableInput() {
            _enableInputCount--;
            UpdateInputState();
        }

        protected void UpdateInputState() {
            if (_enableInputCount > 0) {
                _inputActions.Enable();
            } else {
                _inputActions.Disable();
            }
        }

        protected override void Awake() {
            base.Awake();
            _inputActions = InputSystem.actions.FindActionMap("Character", true);
            UpdateInputState();
        }

        protected void OnDestroy() {
            _inputActions.Disable();
        }
    }
}