using JoG.Localization;
using UnityEngine;
using UnityEngine.Events;

namespace JoG.InteractionSystem {

    public class SimpleInteractable : MonoBehaviour, IInteractable, IInformationProvider {
        public LocalizableString localizableName;
        public LocalizableString localizableDescription;
        public string Name => localizableName;

        public string Description => localizableDescription;
        [field: SerializeField] public UnityEvent OnInteracted { get; private set; }

        public bool CanInteract(Interactor interactor) {
            return true;
        }

        public string GetProperty(string key) {
            throw new System.NotImplementedException();
        }

        public void PreformInteraction(Interactor interactor) {
            OnInteracted.Invoke();
        }
    }
}