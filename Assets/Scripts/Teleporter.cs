using GuestUnion.UI;
using JoG.InteractionSystem;
using JoG.Localization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace JoG {

    public class Teleporter : MonoBehaviour, IInteractable, IWorldTooltipSource {
        public string nextSceneName = string.Empty;
        public Transform tooltipPoint;
        [Inject] internal NetworkManager _networkManager;
        Vector3 IWorldTooltipSource.TooltipPosition => tooltipPoint.position;

        public bool CanInteract(Interactor interactor) {
            return _networkManager.LocalClient.IsSessionOwner;
        }

        public void PreformInteraction(Interactor interactor) {
            _networkManager.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }

        void ITooltipSource.BuildTooltip(TooltipView view) {
            view.Header.SetActive(true);
            view.HeaderText.SetText(Localizer.GetString("teleporter.name"));
            view.Content.SetActive(true);
            view.ContentText.SetText(Localizer.GetString("teleporter.desc", Localizer.GetString("scene." + nextSceneName)));
        }
    }
}