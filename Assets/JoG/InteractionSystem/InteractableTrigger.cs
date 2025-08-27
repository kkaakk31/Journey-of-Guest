using JoG.Localization;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JoG.InteractionSystem {

    public class InteractableTrigger : MonoBehaviour, IInteractable, IInformationProvider {
        public LocalizableString localizableName;
        public LocalizableString localizableDescription;
        public bool triggered;
        [field: SerializeField] public UnityEvent<bool> OnTriggered { get; private set; } = new();
        public string Name => localizableName.Value;

        public string Description => localizableDescription.Value;

        public Interactability GetInteractability(Interactor interactor) {
            return Interactability.Available;
        }

        public string GetProperty(string key) {
            return Localizer.GetString(key);
        }

        public void PreformInteraction(Interactor interactor) {
            triggered = !triggered;
            OnTriggered.Invoke(triggered);
        }
    }
}