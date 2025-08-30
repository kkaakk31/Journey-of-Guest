namespace JoG.InteractionSystem {

    public interface IInteractable {

        bool CanInteract(Interactor interactor);

        void PreformInteraction(Interactor interactor);
    }
}