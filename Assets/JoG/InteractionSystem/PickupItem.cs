using JoG.Character;
using JoG.InventorySystem;
using Unity.Netcode;

namespace JoG.InteractionSystem {

    public class PickupItem : InteractableObject {
        public bool destroyAfterPickup;
        public ItemData itemData;
        public byte count = 1;

        public override bool CanInteract(Interactor interactor) {
            return interactor.TryGetComponent<IItemPickUpController>(out _);
        }

        public override void PreformInteraction(Interactor interactor) {
            if (destroyAfterPickup) {
                NetworkObject.Despawn();
            }
        }
    }
}