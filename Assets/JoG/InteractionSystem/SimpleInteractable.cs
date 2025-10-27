using GuestUnion.TooltipSystem;
using GuestUnion.TooltipSystem.Components;
using JoG.Localization;
using JoG.UI;
using UnityEngine;
using UnityEngine.Events;

namespace JoG.InteractionSystem {

    public class SimpleInteractable : MonoBehaviour, IInteractable, IWorldTooltipSource {
        public LocalizableString localizableName;
        public LocalizableString localizableDescription;

        [field: SerializeField] public UnityEvent OnInteracted { get; private set; }

        public Vector3 TooltipPosition => default;

        public void BuildTooltip(TooltipView view) {
        }

        public bool CanInteract(Interactor interactor) {
            return true;
        }

        public void PreformInteraction(Interactor interactor) {
            OnInteracted.Invoke();
        }
    }
}