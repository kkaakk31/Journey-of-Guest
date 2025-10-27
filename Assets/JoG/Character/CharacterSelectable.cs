using GuestUnion.Extensions.Unity;
using GuestUnion.TooltipSystem;
using GuestUnion.TooltipSystem.Components;
using JoG.InteractionSystem;
using JoG.Localization;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Character {

    public class CharacterSelectable : MonoBehaviour, IInteractable, ITooltipSource {
        public LocalizableString localizableName;
        public LocalizableString localizableDescription;
        [SerializeField] private NetworkObject bodyPrefab;
        public string Name => localizableName.Value;

        public string Description => localizableDescription.Value;

        void ITooltipSource.BuildTooltip(TooltipView view) {
            throw new System.NotImplementedException();
        }

        public bool CanInteract(Interactor interactor) {
            return interactor.TryGetComponent<CharacterBody>(out _);
        }

        void IInteractable.PreformInteraction(Interactor interactor) {
            if (interactor.TryGetComponent<CharacterBody>(out var body)) {
                var master = body.Master.NetworkObject;
                body.Model.transform.GetPositionAndRotation(out var position, out var rotation);
                body.NetworkObject.Despawn();
                var newBody = bodyPrefab.InstantiateAndSpawn(NetworkManager.Singleton,
                     isPlayerObject: true,
                     position: position,
                     rotation: rotation);
                newBody.TrySetParent(master, true);
            } else {
                this.LogWarning("Interactor does not have a CharacterBody component.");
            }
        }
    }
}