using JoG.InteractionSystem;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using VContainer;

namespace JoG {

    public class Teleporter : InteractableObject {
        public string nextSceneName = string.Empty;
        [Inject] private NetworkManager _networkManager;

        public override bool CanInteract(Interactor interactor) {
            return IsSpawned;
        }

        public void Activate() {
            if (_networkManager.LocalClient.IsSessionOwner) {
                _networkManager.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
            }
        }

        public override void PreformInteraction(Interactor interactor) {
            Activate();
        }
    }
}