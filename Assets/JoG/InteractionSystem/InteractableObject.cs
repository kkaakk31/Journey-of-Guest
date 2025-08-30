using JoG.Localization;
using Unity.Netcode;

namespace JoG.InteractionSystem {

    public abstract class InteractableObject : NetworkBehaviour, IInteractable, IInformationProvider {
        public LocalizableString localizableName;
        public LocalizableString localizableDescription;
        public string Name => localizableName.Value;
        public string Description => localizableDescription.Value;

        public string GetProperty(string token) => Localizer.GetString(token);

        public abstract bool CanInteract(Interactor interactor);

        public abstract void PreformInteraction(Interactor interactor);
    }
}