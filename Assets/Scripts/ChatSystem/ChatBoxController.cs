using ANU.IngameDebug.Console;
using EditorAttributes;
using GuestUnion;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;

namespace JoG.ChatSystem {

    public class ChatBoxController : NetworkBehaviour {
        public ushort messageCapacity = 20;
        public TMP_Text messageItemPrefab;
        public float fadeTimer;
        public string playerName;
        public Color32 playerColor;
        private readonly Queue<TMP_Text> messageItems = new();
        [SerializeField] private InputAction _inputAction;
        [field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
        [field: SerializeField] public Transform MessageContainer { get; private set; }
        [field: SerializeField] public TMP_InputField InputField { get; private set; }

        [Inject]
        public void Construct(IAuthenticationService authenticationService) {
            playerName = authenticationService.PlayerName;
            var colorStr = PlayerPrefs.GetString("player_color", "#FFFFFFFF");
            if (ColorUtility.TryParseHtmlString(colorStr, out var color)) {
                playerColor = color;
            } else {
                playerColor = new Color32(255, 255, 255, 255);
            }
        }

        [Button]
        public void AddMessage([OptAltNames("msg")] string message) {
            if (message.IsNullOrEmpty()) return;
            TMP_Text messageItem;
            if (messageItems.Count < messageCapacity) {
                messageItem = Instantiate(messageItemPrefab, MessageContainer);
            } else {
                messageItem = messageItems.Dequeue();
                messageItem.transform.SetAsLastSibling();
            }
            messageItem.text = message;
            messageItems.Enqueue(messageItem);
            fadeTimer = 10f;
        }

        [Button]
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
            InputField.onSelect.AddListener(_ => {
                CursorManager.Instance.RequestShowCursor();
                PlayerCharacterInputer.Instance.ReleaseEnableInput();
                fadeTimer = 3f;
            });
            InputField.onDeselect.AddListener(_ => {
                CursorManager.Instance.ReleaseShowCursor();
                PlayerCharacterInputer.Instance.RequestEnableInput();
            });
            InputField.onSubmit.AddListener(_ => {
                EventSystem.current.SetSelectedGameObject(null);
                if (InputField.text.IsNullOrEmpty()) return;
                SendMessageToEveryone($"{playerName.ToColored(playerColor)}: {InputField.text}");
                InputField.text = string.Empty;
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
            if (InputField.isFocused) {
                CanvasGroup.alpha = 1;
                return;
            }
            if (fadeTimer > 0) {
                fadeTimer -= Time.deltaTime;
            }
            if (fadeTimer.Clamp01() != CanvasGroup.alpha) {
                CanvasGroup.alpha = fadeTimer;
            }
        }

        private void OnEnter(InputAction.CallbackContext context) {
            if (InputField.isFocused) return;
            InputField.ActivateInputField();
        }
    }
}