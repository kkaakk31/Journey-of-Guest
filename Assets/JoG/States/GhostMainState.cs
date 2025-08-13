using JoG.Character;
using JoG.Character.InputBanks;
using JoG.Character.Move;
using JoG.InteractionSystem;
using UnityEngine;

namespace JoG.States {
    public class GhostMainState : State {
        [SerializeField] private RigidbodyCharacterController characterController;
        [SerializeField] private CharacterInteractor interactor;
        [SerializeField] private CharacterBody body;
        [SerializeField] private CharacterModel model;
        private Vector3InputBank moveInput;
        private Vector3InputBank aimInput;
        private TriggerInputBank interactInput;

        protected override void Awake() {
            base.Awake();
            moveInput = body.GetInputBank<Vector3InputBank>("Move");
            aimInput = body.GetInputBank<Vector3InputBank>("Aim");
            interactInput = body.GetInputBank<TriggerInputBank>("Interact");
        }

        protected void OnEnable() {
            characterController.enabled = true;
        }

        protected override void Update() {
            var aimOrigin = body.AimOrigin;
            var aimTarget = aimInput.vector3;
            var aimDirection = (aimTarget - aimOrigin).normalized;
            characterController.UpdateAimDirection(aimDirection); ;
            characterController.UpdateMoveDirection(Quaternion.LookRotation(aimDirection, characterController.CharacterUp) * moveInput.vector3);
            if (interactInput.Triggered && interactor.FindInteractableObject(aimOrigin, aimDirection, out var result)) {
                interactor.Interact(result);
                interactInput.Reset();
            }
            UpdateAnimator();
            base.Update();
        }

        protected void OnDisable() {
            characterController.UpdateMoveDirection(Vector3.zero);
            characterController.enabled = false;
        }

        private void UpdateAnimator() {
            var animator = model.Animator;
            var localV = model.transform.InverseTransformDirection(characterController.currentVelocity) / characterController.maxStableMoveSpeed;
            animator.SetFloat(AnimationParameters.forwardSpeed, localV.z);
            animator.SetFloat(AnimationParameters.rightSpeed, localV.x);
            animator.SetFloat(AnimationParameters.upSpeed, localV.y);
            animator.SetFloat(AnimationParameters.maxMoveSpeed, characterController.maxStableMoveSpeed);
            animator.SetBool(AnimationParameters.isGrounded, characterController.IsStableOnGround);
        }
    }
}
