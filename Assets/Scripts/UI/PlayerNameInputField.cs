using EditorAttributes;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using VContainer;

namespace JoG.UI {

    public class PlayerNameInputField : MonoBehaviour {
        [Inject] private IAuthenticationService _authenticationService;
        [SerializeField, Required] private TMP_InputField _playerNameInputField;

        public async void UpdatePlayerName(string name) {
            var playerName = await _authenticationService.UpdatePlayerNameAsync(name);
            _playerNameInputField.text = playerName;
        }

        private void Awake() {
            _authenticationService.SignedIn += OnSignedIn;
            if (_authenticationService.IsSignedIn) {
                OnSignedIn();
            }
            _playerNameInputField.onEndEdit.AddListener(UpdatePlayerName);
        }

        private void OnDestroy() {
            _authenticationService.SignedIn -= OnSignedIn;
        }

        private void OnSignedIn() {
            _playerNameInputField.text = _authenticationService.PlayerName;
        }
    }
}