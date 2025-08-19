using EditorAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JoG.ChatSystem {

    public class ChatBoxView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        public ushort messageCapacity = 20;
        public TMP_InputField messageItemPrefab;
        private readonly Queue<TMP_InputField> _messageItems = new();
        [SerializeField, Required] private CanvasGroup _canvasGroup;
        [SerializeField, Required] private TMP_InputField _inputField;
        private bool _isPointerOver = false;

        public CanvasGroup CanvasGroup => _canvasGroup;
        public TMP_InputField InputField => _inputField;
        [field: SerializeField, Required] public Transform MessageContainer { get; private set; }

        public TMP_InputField.OnChangeEvent OnInputFieldTextChanged => _inputField.onValueChanged;
        public TMP_InputField.SelectionEvent OnInputFieldSelected => _inputField.onSelect;
        public TMP_InputField.SelectionEvent OnInputFieldDeselected => _inputField.onDeselect;
        public TMP_InputField.SubmitEvent OnInputFieldSubmitted => _inputField.onSubmit;
        public TMP_InputField.SubmitEvent OnInputFieldEndEdit => _inputField.onEndEdit;
        public bool IsPointerOver => _isPointerOver;
        public bool IsInputFieldFocused => _inputField.isFocused;

        public float Alpha {
            get => _canvasGroup.alpha;
            set => _canvasGroup.alpha = value;
        }

        public string InputFieldText {
            get => _inputField.text;
            set => _inputField.text = value;
        }

        [Button(buttonLabel: "AddMessageToChatBox")]
        public void AddMessage(string message) {
            TMP_InputField messageItem;
            if (_messageItems.Count < messageCapacity) {
                messageItem = Instantiate(messageItemPrefab, MessageContainer);
            } else {
                messageItem = _messageItems.Dequeue();
                messageItem.transform.SetAsLastSibling();
            }
            messageItem.text = message;
            _messageItems.Enqueue(messageItem);
        }

        public void SelectInputField() {
            _inputField.ActivateInputField();
        }

        public void DeselectInputField() {
            _inputField.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => _isPointerOver = true;

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => _isPointerOver = false;
    }
}