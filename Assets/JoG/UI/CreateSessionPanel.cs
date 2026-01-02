using GuestUnion.Logging;
using GuestUnion.UI;
using JoG.Localization;
using JoG.Networking;
using JoG.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace JoG.UI {

    public class CreateSessionPanel : MonoBehaviour {
        [Inject] internal IProfileService _profileService;
        [Inject] internal ISessionService _sessionService;
        [Inject] internal PopupManager _popupManager;
        [field: SerializeField] public TMP_InputField SessionNameInputField { get; private set; }
        [field: SerializeField] public TMP_InputField PasswordInputField { get; private set; }
        [field: SerializeField] public TMP_InputField MaxPlayersInputField { get; private set; }
        [field: SerializeField] public Toggle IsPrivateToggle { get; private set; }
        [field: SerializeField] public Button CreateButton { get; private set; }
        [field: SerializeField] public Button ReturnButton { get; private set; }

        private void Awake() {
            CreateButton.onClick.AddListener(CreateSession);
            ReturnButton.onClick.AddListener(() => gameObject.SetActive(false));
            SessionNameInputField.text = _profileService.Nickname + "'s session";
            MaxPlayersInputField.text = "4";
        }

        private async void CreateSession() {
            using (_popupManager.PopupLoader()) {
                try {
                    await _sessionService.CreateSessionAsync(
                         SessionNameInputField.text,
                         PasswordInputField.text,
                         int.Parse(MaxPlayersInputField.text),
                         IsPrivateToggle.isOn
                    );
                } catch (Exception e) {
                    this.LogException(e);
                    var error = Localizer.GetString(L10nKeys.Session.Create.Failed, e.Message);
                    _popupManager.PopupMessage(error, MessageLevel.Error);
                    return;
                }
            }
            var message = Localizer.GetString(L10nKeys.Session.Create.Created, _sessionService.Session.Code);
            GUIUtility.systemCopyBuffer = _sessionService.Session.Code;
            _popupManager.PopupToast(message, MessageLevel.Info, ToastPosition.TopRight);
        }
    }
}