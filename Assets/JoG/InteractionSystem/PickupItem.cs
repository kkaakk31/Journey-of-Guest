using JoG.Character;
using JoG.InventorySystem;
using Unity.Netcode;
using UnityEngine;

namespace JoG.InteractionSystem {

    public class PickupItem : InteractableObject {
        public bool destroyAfterPickup;
        public ItemData itemData;
        public byte count = 1;

        public override Interactability GetInteractability(Interactor interactor) {
            return interactor.TryGetComponent<IItemPickUpController>(out _)
               ? Interactability.Available
               : Interactability.ConditionsNotMet;
        }

        public override void PreformInteraction(Interactor interactor) {
            if (destroyAfterPickup) {
                NetworkObject.Despawn();
            }
        }
    }
}