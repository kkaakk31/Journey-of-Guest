using EditorAttributes;
using JoG.Character;
using JoG.Messages;
using MessagePipe;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using VContainer;

namespace JoG.InventorySystem {

    public class InventoryController : MonoBehaviour, IMessageHandler<CharacterBodyChangedMessage> {
        public Inventory inventory;
        public InputActionReference numberInput;
        public int selectedIndex = 0;
        [Required] public InventoryView view;
        private CharacterBody _body;
        private IDisposable _disposable;
        private ItemUser _itemUser;

        [Button]
        public int AddItem(string itemName, int count) {
            if (ItemCatalog.TryGetItemDef(itemName, out var itemData)) {
                var index = inventory.AddItem(itemData, (short)count);
                view.RefreshSlot(index);
                return index;
            } else {
                return -1;
            }
        }

        [Button]
        public void ExchangeItem(int fromIndex, int toIndex) {
            inventory.ExchangeItem(fromIndex, toIndex);
            view.RefreshSlot(fromIndex);
            view.RefreshSlot(toIndex);
            if (selectedIndex == fromIndex || selectedIndex == toIndex) {
                SelectItem(selectedIndex);
            }
        }

        [Inject]
        public void Construct(IBufferedSubscriber<CharacterBodyChangedMessage> subscriber) {
            _disposable = subscriber.Subscribe(this);
        }

        void IMessageHandler<CharacterBodyChangedMessage>.Handle(CharacterBodyChangedMessage message) {
            _body = message.body;
            if (message.changeType is CharacterBodyChangeType.Get && _body.TryGetComponent(out _itemUser)) {
                enabled = true;
                view.enabled = true;
                _itemUser.Controller = this;
                var item = inventory[selectedIndex];
                _itemUser.Use(item);
            } else {
                enabled = false;
                view.enabled = false;
                if (_itemUser != null) {
                    _itemUser.Controller = null;
                }
                _itemUser = null;
            }
        }

        public int AddItem(ItemData item, short count) {
            var index = inventory.AddItem(item, count);
            view.RefreshSlot(index);
            if (index == selectedIndex) {
                SelectItem(index);
            }
            return index;
        }

        public void AddItem(int index, short count) {
            inventory.AddItem(index, count);
            view.RefreshSlot(index);
            if (index == selectedIndex) {
                SelectItem(index);
            }
        }

        public void RemoveItem(int index, short count) {
            inventory.RemoveItem(index, count);
            view.RefreshSlot(index);
            if (index == selectedIndex) {
                SelectItem(index);
            }
        }

        public int RemoveItem(ItemData item, short count) {
            var index = inventory.RemoveItem(item, count);
            view.RefreshSlot(index);
            if (index == selectedIndex) {
                SelectItem(index);
            }
            return index;
        }

        public void SelectItem(int index) {
            selectedIndex = index;
            _itemUser.Use(inventory[selectedIndex]);
            view.HighlightSlot(selectedIndex);
        }

        private void Awake() {
            inventory = new Inventory(60);
            var inventoryJson = PlayerPrefs.GetString("player_inventory", string.Empty);
            inventory.FromJson(inventoryJson);
        }

        private void OnEnable() {
            numberInput.action.performed += OnNumInput;
            numberInput.action.Enable();
        }

        private void OnDisable() {
            numberInput.action.performed -= OnNumInput;
            numberInput.action.Disable();
        }

        private void OnDestroy() {
            PlayerPrefs.SetString("player_inventory", inventory.ToJson());
            PlayerPrefs.Save();
            _disposable?.Dispose();
        }

        private void OnNumInput(InputAction.CallbackContext context) {
            int idx = -1;
            switch ((context.control as KeyControl).keyCode) {
                case Key.Digit1:
                case Key.Numpad1: idx = 0; break;
                case Key.Digit2:
                case Key.Numpad2: idx = 1; break;
                case Key.Digit3:
                case Key.Numpad3: idx = 2; break;
                case Key.Digit4:
                case Key.Numpad4: idx = 3; break;
                case Key.Digit5:
                case Key.Numpad5: idx = 4; break;
                case Key.Digit6:
                case Key.Numpad6: idx = 5; break;
                case Key.Digit7:
                case Key.Numpad7: idx = 6; break;
                case Key.Digit8:
                case Key.Numpad8: idx = 7; break;
                case Key.Digit9:
                case Key.Numpad9: idx = 8; break;
                case Key.Digit0:
                case Key.Numpad0: idx = 9; break;
            }
            if (idx >= 0) {
                SelectItem(idx);
            }
        }
    }
}