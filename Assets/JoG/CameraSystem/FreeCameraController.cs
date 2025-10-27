using EditorAttributes;
using GuestUnion.Extensions;
using JoG.Character;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoG.CameraSystem {

    public class FreeCameraController : MonoBehaviour, IBodyAttachHandler {
        private float cameraEulerX, cameraEulerY, cameraEulerZ;
        private Transform _trackingTarget;
        private CharacterBody _body;
        private InputAction _lookInput;
        [field: SerializeField, Required] public CinemachineCamera ThirdPersonCamera { get; private set; }

        public void OnBodyAttached(CharacterBody body) {
            _body = body;
            enabled = true;
        }

        public void OnBodyDetached(CharacterBody body) {
            _body = null;
            enabled = false;
        }

        private void Awake() {
            _trackingTarget = ThirdPersonCamera.Follow;
            _lookInput = InputSystem.actions.FindAction("Look", true);
            _lookInput.performed += OnLook;
        }

        private void Update() {
            _trackingTarget.position = _body.AimOrigin;
        }

        private void OnDestroy() {
            _lookInput.performed -= OnLook;
        }

        private void OnLook(InputAction.CallbackContext context) {
            var rotateVector2 = context.ReadValue<Vector2>();
            cameraEulerX = (cameraEulerX + rotateVector2.y).Clamp(-89, 89);
            cameraEulerY = (cameraEulerY + rotateVector2.x).NormalizeAngleOne360();
            _trackingTarget.rotation = Quaternion.Euler(cameraEulerX, cameraEulerY, cameraEulerZ);
        }
    }
}