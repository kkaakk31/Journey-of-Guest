using EditorAttributes;
using GuestUnion.Extensions;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace JoG.CameraSystem {

    public class FreeCameraController : MonoBehaviour {
        private float cameraEulerX, cameraEulerY, cameraEulerZ;
        private Transform _trackingTarget;
        [Inject, Key(Constants.InputAction.Look)] internal InputAction _lookInput;
        [field: SerializeField, Required] public CinemachineCamera ThirdPersonCamera { get; private set; }

        private void Awake() {
            _trackingTarget = ThirdPersonCamera.Follow;
        }

        private void OnEnable() {
            _lookInput.performed += OnLook;
        }

        private void OnDisable() {
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