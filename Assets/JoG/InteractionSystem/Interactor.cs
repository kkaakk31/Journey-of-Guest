using GuestUnion.ObjectPool.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

namespace JoG.InteractionSystem {

    public class Interactor : NetworkBehaviour {
        public event Action<IInteractable> OnInteract;

        public void Interact(GameObject interactableObject) {
            using (ListPool<IInteractable>.Rent(out var interactables)) {
                interactableObject.GetComponents(interactables);
                if (interactables.Count == 0) return;
                foreach (var interactable in interactables) {
                    if (interactable.CanInteract(this)) {
                        OnInteract.Invoke(interactable);
                        interactable.PreformInteraction(this);
                    }
                }
            }
        }
    }
}