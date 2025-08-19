using EditorAttributes;
using GuestUnion;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;

namespace JoG.ChatSystem {

    public class ChatBoxController : NetworkBehaviour {
        public float fadeTimer;
        public string senderName;
        public Color32 snederColor;
        [SerializeField] private InputAction _inputAction;
        [field: SerializeField] public ChatBoxView View { get; private set; }

        [Inject]
        public void Construct(IAuthenticationService authenticationService) {
            senderName = authenticationService.PlayerName[..^5];
            var colorStr = PlayerPrefs.GetString("player_color", "#FFFFFFFF");
            if (ColorUtility.TryParseHtmlString(colorStr, out var color)) {
                snederColor = color;
            } else {
                snederColor = new Color32(255, 255, 255, 255);
            }
        }

        public void AddMessage(string message) {
            if (message.IsNullOrWhiteSpace()) return;
            View.AddMessage(message);
            fadeTimer = 10f;
        }

        public void SendMessageToEveryone(string message) {
            if (message.IsNullOrEmpty()) return;
            SendMessageRpc(message);
        }

        public override void OnNetworkSpawn() {
            _inputAction.Enable();
        }

        public override void OnNetworkDespawn() {
            _inputAction.Disable();
        }

        public void Awake() {
            View.OnInputFieldSelected.AddListener(_ => {
                CursorManager.Instance.ShowCursor();
                CharacterInputManager.Instance.DisableInput();
                fadeTimer = 3f;
            });
            View.OnInputFieldDeselected.AddListener(_ => {
                CursorManager.Instance.HideCursor();
                CharacterInputManager.Instance.EnableInput();
            });
            View.OnInputFieldSubmitted.AddListener(text => {
                View.DeselectInputField();
                if (text.IsNullOrWhiteSpace()) return;
                SendMessageToEveryone($"{senderName.ToColored(snederColor)}: {text}");
                View.InputFieldText = string.Empty;
            });
            _inputAction.performed += OnEnter;
        }

        public override void OnDestroy() {
            base.OnDestroy();
            _inputAction.performed -= OnEnter;
            _inputAction.Dispose();
        }

        [Rpc(SendTo.Everyone)]
        private void SendMessageRpc(string message) => AddMessage(message);

        private void Update() {
            if (View.IsPointerOver || View.IsInputFieldFocused) {
                View.Alpha = 1f;
                return;
            }
            if (fadeTimer > 0) {
                fadeTimer -= Time.deltaTime;
            }
            View.Alpha = fadeTimer;
        }

        private void OnEnter(InputAction.CallbackContext context) {
            if (View.IsInputFieldFocused) return;
            View.SelectInputField();
        }
    }
}