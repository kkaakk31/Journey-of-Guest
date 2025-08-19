using JoG.Character.InputBanks;
using JoG.Messages;
using MessagePipe;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace JoG {

    public class PlayerCharacterInputer : MonoBehaviour, IMessageHandler<CharacterBodyChangedMessage> {
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
        private IDisposable _disposable;
        private int _enableInputCount;

        void IMessageHandler<CharacterBodyChangedMessage>.Handle(CharacterBodyChangedMessage message) {
            var body = message.body;
            if (message.changeType is CharacterBodyChangeType.Get) {
                interactInputBank = body.GetInputBank<TriggerInputBank>("Interact");
                jumpInputBank = body.GetInputBank<TriggerInputBank>("Jump");
                moveInputBank = body.GetInputBank<Vector3InputBank>("Move");
                primaryActionInputBank = body.GetInputBank<TriggerInputBank>("PrimaryAction");
                secondaryActionInputBank = body.GetInputBank<TriggerInputBank>("SecondaryAction");
                skillInputBank = body.GetInputBank<TriggerInputBank>("Skill");
                sprintInputBank = body.GetInputBank<BooleanInputBank>("Sprint");
                RegisterCallback();
                CharacterInputManager.Instance.EnableInput();
                CursorManager.Instance.HideCursor();
            } else if (message.changeType is CharacterBodyChangeType.Lose) {
                interactInputBank = null;
                jumpInputBank = null;
                moveInputBank = null;
                primaryActionInputBank = null;
                secondaryActionInputBank = null;
                skillInputBank = null;
                sprintInputBank = null;
                UnregisterCallback();
                CharacterInputManager.Instance.DisableInput();
                CursorManager.Instance.ShowCursor();
            }
        }

        [Inject]
        private void Construct(InputActionAsset inputActionAsset, IBufferedSubscriber<CharacterBodyChangedMessage> subscriber) {
            _characterActionMap = inputActionAsset.FindActionMap("Character", true);
            _move = _characterActionMap.FindAction("Move", true);
            _primaryAction = _characterActionMap.FindAction("PrimaryAction", true);
            _secondaryAction = _characterActionMap.FindAction("SecondaryAction", true);
            _jump = _characterActionMap.FindAction("Jump", true);
            _sprint = _characterActionMap.FindAction("Sprint", true);
            _skill = _characterActionMap.FindAction("Skill", true);
            _interact = _characterActionMap.FindAction("Interact", true);
            _disposable = subscriber.Subscribe(this);
        }

        private void OnDestroy() {
            _characterActionMap?.Disable();
            _disposable?.Dispose();
        }

        private void RegisterCallback() {
            _move.performed += OnMove;
            _move.canceled += OnMove;
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
            _move.performed -= OnMove;
            _move.canceled -= OnMove;
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

        private void OnMove(InputAction.CallbackContext context) {
            var moveInput = context.ReadValue<Vector2>();
            moveInputBank.vector3 = new Vector3(moveInput.x, 0, moveInput.y);
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