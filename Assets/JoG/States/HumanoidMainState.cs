using GuestUnion.Extensions.Unity;
using JoG.Character;
using JoG.Character.Controller;
using JoG.Character.InputBanks;
using JoG.InteractionSystem;
using UnityEngine;

namespace JoG.States {

    public class HumanoidMainState : State {

        [Header("Movement")]
        public float baseSpeedLimit = 7f;

        public float acceleration = 70f;
        public float deceleration = 40f;
        public float airAccelerationMultiplier = 0.4f;
        public float unstableGroundAccelerationMultiplier = 0.7f;

        [Header("Rotation")]
        public float rotateSpeed = 10f;

        public bool rotateToMovementDirection = true;

        [SerializeField] private CharacterActor characterController;
        [SerializeField] private CharacterInteractor interactor;
        [SerializeField] private CharacterBody body;
        [SerializeField] private CharacterModel model;

        // 输入银行
        private Vector3InputBank moveInput;
        private Vector3InputBank aimInput;
        private TriggerInputBank jumpInput;
        private TriggerInputBank interactInput;
        private BooleanInputBank sprintInput;
        private BooleanInputBank crouchInput;

        private float _currentPlanarSpeedLimit = 0f;

        protected override void Awake() {
            base.Awake();
            moveInput = body.GetInputBank<Vector3InputBank>("Move");
            aimInput = body.GetInputBank<Vector3InputBank>("Aim");
            jumpInput = body.GetInputBank<TriggerInputBank>("Jump");
            interactInput = body.GetInputBank<TriggerInputBank>("Interact");
            sprintInput = body.GetInputBank<BooleanInputBank>("Sprint");
            crouchInput = body.GetInputBank<BooleanInputBank>("Crouch");

            if (model != null && model.Animator != null) {
                model.Animator.logWarnings = false;
            }

            _currentPlanarSpeedLimit = baseSpeedLimit;
        }

        protected void OnEnable() {
            if (characterController != null) {
                characterController.enabled = true;
            }
        }

        protected override void Update() {
            HandleRotation();
            HandleInteraction();
            UpdateAnimator();
            base.Update();
        }

        protected void FixedUpdate() {
            HandleMovement();
        }

        protected void OnDisable() {
            if (characterController != null) {
                characterController.enabled = false;
            }
        }

        private void HandleRotation() {
            if (!rotateToMovementDirection) return;

            var moveDirection = moveInput.vector3.ProjectOnPlane(characterController.Up);

            // 根据是否有移动输入决定旋转行为
            if (moveDirection.sqrMagnitude > 0.1f) {
                moveDirection.Normalize();

                // 旋转角色面向移动方向
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, characterController.Up);
                characterController.Rotation = Quaternion.Slerp(
                    characterController.Rotation,
                    targetRotation,
                    Time.deltaTime * rotateSpeed
                );
            }
        }

        private void HandleMovement() {
            var moveDirection = moveInput.vector3.ProjectOnPlane(characterController.Up);
            if (moveDirection.sqrMagnitude > 0.1f) {
                moveDirection.Normalize();
            }

            Vector3 targetPlanarVelocity = Vector3.zero;
            float currentAcceleration = acceleration;
            float currentDeceleration = deceleration;

            // 根据角色当前状态调整移动行为
            switch (characterController.CurrentState) {
                case CharacterState.NotGrounded:
                    // 空中控制
                    if (characterController.WasGrounded) {
                        // 刚起跳时保持当前水平速度
                        _currentPlanarSpeedLimit = Mathf.Max(characterController.PlanarVelocity.magnitude, baseSpeedLimit);
                    }
                    targetPlanarVelocity = moveDirection * _currentPlanarSpeedLimit;
                    currentAcceleration *= airAccelerationMultiplier;
                    currentDeceleration *= airAccelerationMultiplier;
                    break;

                case CharacterState.StableGrounded:
                    // 稳定地面移动
                    targetPlanarVelocity = moveDirection * _currentPlanarSpeedLimit;
                    break;

                case CharacterState.UnstableGrounded:
                    // 不稳定地面移动（如陡坡）
                    targetPlanarVelocity = moveDirection * _currentPlanarSpeedLimit;
                    currentAcceleration *= unstableGroundAccelerationMultiplier;
                    currentDeceleration *= unstableGroundAccelerationMultiplier;
                    break;
            }

            // 根据是加速还是减速选择相应的参数
            bool needToAccelerate = targetPlanarVelocity.sqrMagnitude >= characterController.PlanarVelocity.sqrMagnitude;
            float accelerationValue = needToAccelerate ? currentAcceleration : currentDeceleration;

            // 平滑插值到目标速度
            var currentVelocity = characterController.PlanarVelocity;
            var newVelocity = Vector3.MoveTowards(currentVelocity, targetPlanarVelocity, accelerationValue * Time.deltaTime);
            characterController.PlanarVelocity = newVelocity;
        }

        private void HandleInteraction() {
            var aimOrigin = body.AimOrigin;
            var aimTarget = aimInput.vector3;
            var aimDirection = (aimTarget - aimOrigin).normalized;

            if (interactInput.Triggered && interactor.FindInteractableObject(aimOrigin, aimDirection, out var result)) {
                interactor.Interact(result);
                interactInput.Reset();
            }
        }

        private void UpdateAnimator() {
            if (model == null || model.Animator == null) return;

            var animator = model.Animator;
            var localVelocity = characterController.LocalVelocity / _currentPlanarSpeedLimit;

            animator.SetFloat(AnimationParameters.forwardSpeed, localVelocity.z);
            animator.SetFloat(AnimationParameters.rightSpeed, localVelocity.x);
            animator.SetFloat(AnimationParameters.upSpeed, localVelocity.y);
            animator.SetFloat(AnimationParameters.maxMoveSpeed, _currentPlanarSpeedLimit);
            animator.SetBool(AnimationParameters.isGrounded, characterController.IsStable);
            //animator.SetBool(AnimationParameters.isSprinting, wantToRun);
            //animator.SetBool(AnimationParameters.isCrouching, isCrouched);
        }
    }
}