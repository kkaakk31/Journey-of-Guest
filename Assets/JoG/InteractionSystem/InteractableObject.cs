using GuestUnion.TooltipSystem;
using GuestUnion.TooltipSystem.Components;
using Unity.Netcode;

namespace JoG.InteractionSystem {

    public abstract class InteractableObject : NetworkBehaviour, IInteractable, ITooltipSource {

        public abstract bool CanInteract(Interactor interactor);

        public abstract void PreformInteraction(Interactor interactor);

        public abstract void BuildTooltip(TooltipView view);
    }
}