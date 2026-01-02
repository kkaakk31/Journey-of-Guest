using GuestUnion.Logging;
using GuestUnion.UI;
using JoG.Localization;
using JoG.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace JoG.UI {

    public class JoinSessionPanel : MonoBehaviour {
        [Inject] internal ISessionService _sessionService;
        [Inject] internal PopupManager PopupManager;
        [field: SerializeField] public TMP_InputField SessionCodeInputField { get; private set; }
        [field: SerializeField] public TMP_InputField PasswordInputField { get; private set; }
        [field: SerializeField] public Button JoinButton { get; private set; }
        [field: SerializeField] public Button ReturnButton { get; private set; }

        private void Awake() {
            JoinButton.onClick.AddListener(JoinSession);
            ReturnButton.onClick.AddListener(() => gameObject.SetActive(false));
            SessionCodeInputField.text = GUIUtility.systemCopyBuffer;
        }

        private async void JoinSession() {
            using (PopupManager.PopupLoader()) {
                try {
                    var sessionCode = SessionCodeInputField.text;
                    var password = PasswordInputField.text;
                    await _sessionService.JoinSessionByCodeAsync(sessionCode, password);
                } catch (System.Exception e) {
                    this.LogException(e);
                    var error = Localizer.GetString(L10nKeys.Session.Join.Failed, e.Message);
                    PopupManager.PopupMessage(error, MessageLevel.Error);
                }
            }
        }
    }
}