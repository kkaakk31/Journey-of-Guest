using GuestUnion.ObjectPool.Generic;
using JoG.InteractionSystem;
using JoG.InventorySystem;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Character {

    public class ItemUser : MonoBehaviour, IItemPickUpController {
        private InventoryItem currentItem;
        private NetworkObject currentItemObject;
        public List<IItemHandler> Handlers { get; private set; }
        public InventoryController Controller { get; set; }

        public void ChangeItemCount(short changeValue) {
            if (currentItem.index < 0 || changeValue is 0) {
                return;
            }
            if (changeValue > 0) {
                Controller.AddItem(currentItem.index, changeValue);
            } else {
                Controller.RemoveItem(currentItem.index, (short)-changeValue);
            }
        }

        public void Use(in InventoryItem item) {
            currentItem = item;
            if (currentItemObject != null) {
                if (currentItemObject.IsSpawned) {
                    currentItemObject.Despawn();
                } else {
                    Destroy(currentItemObject);
                }
            }
            NetworkObject spawned = null;
            if (item.Prefab != null && item.Prefab.TryGetComponent(out NetworkObject prefab)) {
                spawned = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(prefab, destroyWithScene: true);
            }
            currentItemObject = spawned;
            foreach (var handler in Handlers.AsSpan()) {
                handler.Handle(spawned.gameObject);
            }
        }

        void IInteractionMessageHandler.Handle(IInteractable interactableObject) {
            if (interactableObject is PickupItem pickupItem) {
                Controller.AddItem(pickupItem.itemData, pickupItem.count);
            }
        }

        protected virtual void Awake() {
            Handlers = ListPool<IItemHandler>.shared.Rent();
            GetComponentsInChildren(true, Handlers);
        }

        protected virtual void OnDisable() {
            Use(default);
        }

        protected virtual void OnDestroy() {
            ListPool<IItemHandler>.shared.Return(Handlers);
        }
    }
}