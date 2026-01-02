using EditorAttributes;
using JoG.Character;
using JoG.InteractionSystem;
using JoG.Item;
using JoG.Item.Datas;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace JoG.InventorySystem {

    public class InventoryController : MonoBehaviour, INetworkBehaviour {
        [Required, SerializeField] private CharacterInventory _inventory;
        [Required, SerializeField] private InventoryTableView _tableView;
        [Required, SerializeField] private BuffCollection _buffCollection;
        [Required, SerializeField] private Interactor _interactor;
        [Inject, Key(Constants.InputAction.Inventory)] internal InputAction _tableToggle;

        void INetworkBehaviour.OnSpawn() {
            foreach (var item in _inventory.Items) {
                foreach (var buffData in item.ItemData.buffDatas) {
                    var buff = buffData.RentItemBuff(item.ItemCount);
                    _buffCollection.AddBuffOnEveryone(buff);
                }
            }
            _interactor.OnInteract += OnBodyInteract;
            _inventory.OnItemCountChanged += OnInventoryItemCountChanged;
            _tableToggle.performed += OnTableToggle;
        }

        void INetworkBehaviour.OnDespawn() {
            _interactor.OnInteract -= OnBodyInteract;
            _inventory.OnItemCountChanged -= OnInventoryItemCountChanged;
            _tableToggle.performed -= OnTableToggle;
        }

        private void OnTableToggle(InputAction.CallbackContext callback) {
            if (_tableView.IsVisible) {
                _tableView.Hide();
            } else {
                _tableView.Show();
            }
        }

        private void OnBodyInteract(IInteractable interactableObject) {
            if (interactableObject is ItemPickup pickup) {
                pickup.TryPickup(NetworkManager.Singleton.LocalClientId, OnPickup);
            }
        }

        private void OnPickup(ItemData itemData, int amount) {
            _inventory.AddItem(itemData, amount);
        }

        private void OnInventoryItemCountChanged(ItemData itemData, int itemCount) {
            if (itemCount > 0) {
                foreach (var buffData in itemData.buffDatas) {
                    var buff = buffData.RentItemBuff(itemCount);
                    _buffCollection.AddBuffOnEveryone(buff);
                }
            } else {
                foreach (var buffData in itemData.buffDatas) {
                    _buffCollection.RemoveBuffOnEveryone(buffData.index);
                }
            }
        }
    }
}