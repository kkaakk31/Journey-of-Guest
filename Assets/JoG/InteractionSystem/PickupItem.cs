using EditorAttributes;
using JoG.Character;
using JoG.InventorySystem;
using Unity.Netcode;

namespace JoG.InteractionSystem {

    public class PickupItem : NetworkBehaviour, IInteractable, IInformationProvider {
        [PropertyDropdown, Required] public ItemData itemData;
        public byte count = 1;
        public string Name => itemData?.Name;

        public string Description => itemData?.Description;

        public bool CanInteract(Interactor interactor) {
            return interactor.TryGetComponent<IItemPickUpController>(out _);
        }

        public string GetProperty(string key) {
            throw new System.NotImplementedException();
        }

        public void PreformInteraction(Interactor interactor) {
            if (count == 0) {
                NetworkObject.Despawn();
            }
        }
    }
}