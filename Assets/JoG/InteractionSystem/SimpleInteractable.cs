using GuestUnion.UI;
using JoG.Localization;
using UnityEngine;
using UnityEngine.Events;

namespace JoG.InteractionSystem {

    public class SimpleInteractable : MonoBehaviour, IInteractable, IWorldTooltipSource {
        public LocalizableString localizableName;
        public LocalizableString localizableDescription;
        public Vector3 tooltipOffset;
        [field: SerializeField] public UnityEvent OnInteracted { get; private set; } = new();

        public Vector3 TooltipPosition => transform.position + tooltipOffset;

        public void BuildTooltip(TooltipView view) {
            view.Header.SetActive(true);
            view.Content.SetActive(true);
            view.HeaderText.SetText(localizableName.Value);
            view.ContentText.SetText(localizableDescription.Value);
        }

        public bool CanInteract(Interactor interactor) => true;

        public void PreformInteraction(Interactor interactor) {
            OnInteracted.Invoke();
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + tooltipOffset);
            Gizmos.DrawSphere(transform.position + tooltipOffset, 0.05f);
        }
    }
}