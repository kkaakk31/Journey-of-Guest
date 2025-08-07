using JoG.Character;
using JoG.InteractionSystem;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Assets.JoG.Character {

    public class CharacterSelectable : MonoBehaviour, IInteractable {
        [SerializeField] private NetworkObject bodyPrefab;

        Interactability IInteractable.GetInteractability(Interactor interactor) {
            return interactor.TryGetComponent<CharacterBody>(out _)
                ? Interactability.Available
                : Interactability.ConditionsNotMet;
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
                Debug.LogWarning("Interactor does not have a CharacterBody component.");
            }
        }
    }
}