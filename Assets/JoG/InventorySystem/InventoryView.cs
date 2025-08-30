using EditorAttributes;
using GuestUnion;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoG.InventorySystem {

    public class InventoryView : MonoBehaviour {
        [Required] public InventoryController controller;
        [Required] public GameObject inventoryRoot;
        [Required] public GameObject tablePanel;
        [Required] public GameObject hotBarPanel;
        public DragItem dragItem;
        public Color selectedColor = Color.yellow;
        public Color normalColor = Color.white;
        [SerializeField, Required] private InputActionReference _tableToggle;
        private List<Slot> slots = new();
        private Slot selectedSlot;

        public void RefreshAllSlots() {
            foreach (var slot in slots.AsSpan()) {
                slot.UpdateView(controller.inventory[slot.index]);
            }
        }

        public void RefreshSlot(int index) {
            if (index >= 0 && index < slots.Count)
                slots[index].UpdateView(controller.inventory[index]);
        }

        public void HighlightSlot(int selectedIndex) {
            if (selectedSlot != null) {
                selectedSlot.slotImage.color = normalColor;
            }
            if (slots.TryGetAt(selectedIndex, out selectedSlot)) {
                selectedSlot.slotImage.color = selectedColor;
            }
        }

        public void ShowDragItem(Sprite iconSprite, string countText) {
            dragItem.iconImage.sprite = iconSprite;
            dragItem.countText.text = countText;
            dragItem.gameObject.SetActive(true);
        }

        public void SetDragItemPosition(Vector2 position) {
            dragItem.transform.localPosition = position;
        }

        public void HideDragItem() {
            dragItem.gameObject.SetActive(false);
        }

        public void ExchangeItem(int from, int to) {
            controller.ExchangeItem(from, to);
        }

        private void Awake() {
            GetComponentsInChildren(true, slots);
            for (var i = 0; i < slots.Count; i++) {
                var slot = slots[i];
                slot.view = this;
                slot.index = i;
            }
        }

        private void OnEnable() {
            inventoryRoot.SetActive(true);
            tablePanel.SetActive(false);
            hotBarPanel.SetActive(true);
            RefreshAllSlots();
            HighlightSlot(controller.selectedIndex);
            _tableToggle.action.performed += OnUIToggle;
            _tableToggle.action.Enable();
        }

        private void OnDisable() {
            if (tablePanel.activeSelf) {
                tablePanel.SetActive(false);
                CharacterInputManager.Instance.EnableInput();
                CursorManager.Instance.HideCursor();
            }
            inventoryRoot.SetActive(false);
            _tableToggle.action.Disable();
            _tableToggle.action.performed -= OnUIToggle;
        }

        private void OnUIToggle(InputAction.CallbackContext callback) {
            if (tablePanel.activeSelf) {
                tablePanel.SetActive(false);
                CharacterInputManager.Instance.EnableInput();
                CursorManager.Instance.HideCursor();
            } else {
                tablePanel.SetActive(true);
                CharacterInputManager.Instance.DisableInput();
                CursorManager.Instance.ShowCursor();
            }
        }
    }
}