using EditorAttributes;
using JoG.Character;
using JoG.Character.InputBanks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace JoG {

    [DefaultExecutionOrder(-10)]
    public class PlayerCharacterInputer : MonoBehaviour {
        public LayerMask aimCollisionFilter;
        [Inject] internal InputBankProvider _inputBankProvider;
        [Inject, Key(Constants.InputAction.Move)] internal InputAction _move;
        [Inject, Key(Constants.InputAction.PrimaryAction)] internal InputAction _primaryAction;
        [Inject, Key(Constants.InputAction.SecondaryAction)] internal InputAction _secondaryAction;
        [Inject, Key(Constants.InputAction.Jump)] internal InputAction _jump;
        [Inject, Key(Constants.InputAction.Sprint)] internal InputAction _sprint;
        [Inject, Key(Constants.InputAction.Skill)] internal InputAction _skill;
        [Inject, Key(Constants.InputAction.Interact)] internal InputAction _interact;
        private BooleanInputBank sprintInputBank;
        private TriggerInputBank interactInputBank;
        private TriggerInputBank jumpInputBank;
        private TriggerInputBank primaryActionInputBank;
        private TriggerInputBank secondaryActionInputBank;
        private TriggerInputBank skillInputBank;
        private Vector3InputBank _moveInputBank;
        private Vector3InputBank _aimInputBank;
        [field: SerializeField, Required] public CinemachineCamera PlayerCharacterCamera { get; private set; }

        private void Awake() {
            interactInputBank = _inputBankProvider.GetInputBank<TriggerInputBank>("Interact");
            jumpInputBank = _inputBankProvider.GetInputBank<TriggerInputBank>("Jump");
            _moveInputBank = _inputBankProvider.GetInputBank<Vector3InputBank>("Move");
            primaryActionInputBank = _inputBankProvider.GetInputBank<TriggerInputBank>("PrimaryAction");
            secondaryActionInputBank = _inputBankProvider.GetInputBank<TriggerInputBank>("SecondaryAction");
            skillInputBank = _inputBankProvider.GetInputBank<TriggerInputBank>("Skill");
            sprintInputBank = _inputBankProvider.GetInputBank<BooleanInputBank>("Sprint");
            _aimInputBank = _inputBankProvider.GetInputBank<Vector3InputBank>("Aim");
        }

        private void OnEnable() {
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

        private void Update() {
            var state = PlayerCharacterCamera.State;
            var origin = state.GetFinalPosition();
            var rotation = state.GetFinalOrientation();
            var direction = rotation * Vector3.forward;
            if (Physics.Raycast(origin, direction, out var hit, 1000, aimCollisionFilter, QueryTriggerInteraction.Ignore)) {
                _aimInputBank.vector3 = hit.point;
            } else {
                _aimInputBank.vector3 = origin + (1000 * direction);
            }
            var moveInput = _move.ReadValue<Vector2>();
            _moveInputBank.vector3 = rotation * new Vector3(moveInput.x, 0, moveInput.y);
        }

        private void OnDisable() {
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