using JoG.InteractionSystem;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using VContainer;

namespace JoG {

    public class Teleporter : InteractableObject {
        public string nextSceneName = string.Empty;
        [Inject] private NetworkManager _networkManager;

        public override Interactability GetInteractability(Interactor interactor) {
            return enabled
                ? interactor.CompareTag("Player") ? Interactability.Available : Interactability.ConditionsNotMet
                : Interactability.Disabled;
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