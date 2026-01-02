using JoG.Character;
using JoG.Character.InputBanks;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace JoG.InteractionSystem {

    public class CharacterInteractor : Interactor {
        public LayerMask interactiveLayer;
        [Range(0, float.MaxValue)] public float maxDistance = 3f;
        [SerializeField] public InputBankProvider _inputBankCollection;
        private Vector3InputBank _aimInputBank;

        public void FindAndInteract(in Vector3 origin, in Vector3 direction) {
            if (FindInteractableObject(origin, direction, out var result)) {
                Interact(result);
            }
        }

        public bool FindInteractableObject(in Vector3 origin, in Vector3 direction, [NotNullWhen(true)] out GameObject result) {
            if (Physics.Raycast(origin, direction, out var hitInfo, maxDistance, interactiveLayer, QueryTriggerInteraction.Collide)) {
                result = hitInfo.collider.gameObject;
                return true;
            }
            result = null;
            return false;
        }

        protected void Reset() {
            interactiveLayer = LayerMask.GetMask("Default", "Interactable");
        }
    }
}