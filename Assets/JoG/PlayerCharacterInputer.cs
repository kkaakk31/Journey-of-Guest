using EditorAttributes;
using JoG.Character;
using JoG.Character.InputBanks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoG {

    public class PlayerCharacterInputer : MonoBehaviour, IBodyAttachHandler {
        public LayerMask aimCollisionFilter;
        private BooleanInputBank sprintInputBank;
        private TriggerInputBank interactInputBank;
        private TriggerInputBank jumpInputBank;
        private TriggerInputBank primaryActionInputBank;
        private TriggerInputBank secondaryActionInputBank;
        private TriggerInputBank skillInputBank;
        private Vector3InputBank moveInputBank;
        private InputActionMap _characterActionMap;
        private InputAction _move;
        private InputAction _primaryAction;
        private InputAction _secondaryAction;
        private InputAction _jump;
        private InputAction _sprint;
        private InputAction _skill;
        private InputAction _interact;
        private Vector3InputBank _aimInputBank;
        [field: SerializeField, Required] public CinemachineCamera PlayerCharacterCamera { get; private set; }

        public void OnBodyAttached(CharacterBody body) {
            interactInputBank = body.GetInputBank<TriggerInputBank>("Interact");
            jumpInputBank = body.GetInputBank<TriggerInputBank>("Jump");
            moveInputBank = body.GetInputBank<Vector3InputBank>("Move");
            primaryActionInputBank = body.GetInputBank<TriggerInputBank>("PrimaryAction");
            secondaryActionInputBank = body.GetInputBank<TriggerInputBank>("SecondaryAction");
            skillInputBank = body.GetInputBank<TriggerInputBank>("Skill");
            sprintInputBank = body.GetInputBank<BooleanInputBank>("Sprint");
            _aimInputBank = body.GetInputBank<Vector3InputBank>("Aim");
            RegisterCallback();
            CharacterInputManager.Instance.EnableInput();
            CursorManager.Instance.HideCursor();
            enabled = true;
        }

        public void OnBodyDetached(CharacterBody body) {
            interactInputBank = null;
            jumpInputBank = null;
            moveInputBank = null;
            primaryActionInputBank = null;
            secondaryActionInputBank = null;
            skillInputBank = null;
            sprintInputBank = null;
            _aimInputBank = null;
            UnregisterCallback();
            CharacterInputManager.Instance.DisableInput();
            CursorManager.Instance.ShowCursor();
            enabled = false;
        }

        private void Awake() {
            _characterActionMap = InputSystem.actions.FindActionMap("Character", true);
            _move = _characterActionMap.FindAction("Move", true);
            _primaryAction = _characterActionMap.FindAction("PrimaryAction", true);
            _secondaryAction = _characterActionMap.FindAction("SecondaryAction", true);
            _jump = _characterActionMap.FindAction("Jump", true);
            _sprint = _characterActionMap.FindAction("Sprint", true);
            _skill = _characterActionMap.FindAction("Skill", true);
            _interact = _characterActionMap.FindAction("Interact", true);
        }

        private void Update() {
            var state = PlayerCharacterCamera.State;
            var origin = state.GetFinalPosition();
            var rotation = state.GetFinalOrientation();
            var direction = rotation * Vector3.forward;
            var moveInput = _move.ReadValue<Vector2>();
            moveInputBank.vector3 = rotation * new Vector3(moveInput.x, 0, moveInput.y);
            if (Physics.Raycast(origin, direction, out var hit, 1000, aimCollisionFilter, QueryTriggerInteraction.Ignore)) {
                _aimInputBank.vector3 = hit.point;
            } else {
                _aimInputBank.vector3 = origin + (1000 * direction);
            }
        }

        private void OnDestroy() {
            _characterActionMap.Disable();
        }

        private void RegisterCallback() {
            _primaryAction.performed += OnPrimaryAction;
            _primaryAction.canceled += OnPrimaryAction;
            _secondaryAction.performed += OnSecondaryAction;
            _secondaryAction.canceled += OnSecondaryAction;
            _jump.performed += OnJump;
            _jump.canceled += OnJump;
            _sprint.performed += OnSprint;
            _interact.performed += OnInteract;
            _interact.canceled += OnInteract;
            _skill.performed += OnSkill;
            _skill.canceled += OnSkill;
        }

        private void UnregisterCallback() {
            _primaryAction.performed -= OnPrimaryAction;
            _primaryAction.canceled -= OnPrimaryAction;
            _secondaryAction.performed -= OnSecondaryAction;
            _secondaryAction.canceled -= OnSecondaryAction;
            _jump.performed -= OnJump;
            _jump.canceled -= OnJump;
            _sprint.performed -= OnSprint;
            _interact.performed -= OnInteract;
            _interact.canceled -= OnInteract;
            _skill.performed -= OnSkill;
            _skill.canceled -= OnSkill;
        }

        private void OnPrimaryAction(InputAction.CallbackContext context) {
            primaryActionInputBank.UpdateState(context.performed);
        }

        private void OnSecondaryAction(InputAction.CallbackContext context) {
            secondaryActionInputBank.UpdateState(context.performed);
        }

        private void OnJump(InputAction.CallbackContext context) {
            jumpInputBank.UpdateState(context.performed);
        }

        private void OnSprint(InputAction.CallbackContext context) {
            sprintInputBank.UpdateState(!sprintInputBank.Value);
        }

        private void OnInteract(InputAction.CallbackContext context) {
            interactInputBank.UpdateState(context.performed);
        }

        private void OnSkill(InputAction.CallbackContext context) {
            skillInputBank.UpdateState(context.performed);
        }
    }
}