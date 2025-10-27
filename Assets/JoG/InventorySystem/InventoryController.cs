using EditorAttributes;
using JoG.Character;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace JoG.InventorySystem {

    public class InventoryController : MonoBehaviour,IBodyAttachHandler {
        [ReadOnly] public int selectedIndex = 0;
        [SerializeField, Required] private InputActionReference _numberInput;
        [SerializeField, Required] private InputActionReference _tableToggle;
        [SerializeField, Required] private InventoryTableView _tableView;
        private CharacterBody _body;
        private IInventory _inventory;
        private IItemUser _itemUser;

        public void OnBodyAttached(CharacterBody body) {
            _body = body;
            if (_body.TryGetComponent(out _itemUser)) {
                _itemUser.Inventory = _inventory;
                SelectItem(selectedIndex);
            }
            _numberInput.action.performed += OnNumInput;
            _tableToggle.action.performed += OnTableToggle;
            _numberInput.action.Enable();
            _tableToggle.action.Enable();
        }

        public void OnBodyDetached(CharacterBody body) {
            _body = null;
            if (_itemUser != null) {
                _itemUser.Inventory = null;
            }
            _itemUser = null;
            _numberInput.action.performed -= OnNumInput;
            _tableToggle.action.performed -= OnTableToggle;
            _numberInput.action.Disable();
            _tableToggle.action.Disable();
        }

        public void SelectItem(int index) {
            selectedIndex = index;
            _itemUser.Use(_inventory[index]);
            //view.HighlightSlot(index);
        }

        protected void Awake() {
            _inventory = GetComponent<Inventory>();
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

        private void OnTableToggle(InputAction.CallbackContext callback) {
            if (_tableView.IsVisible) {
                _tableView.Hide();
            } else {
                _tableView.Show();
            }
        }
    }
}