using EditorAttributes;
using GuestUnion;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoG.InventorySystem {

    public class InventoryUIController : MonoBehaviour {

        [Header("依赖组件")]
        [Required] public InventoryController inventoryController;
        public GameObject inventoryRoot;
        public GameObject tablePanel;
        public GameObject hotBarPanel;
        public DragItem dragItem;
        public Color selectedColor = Color.yellow;
        public Color normalColor = Color.white;
        [SerializeField, Required] private InputActionReference _uiToggle;
        private List<Slot> slots = new();
        private Slot selectedSlot;

        public void RefreshAllSlots() {
            var span = slots.AsSpan();
            for (var i = 0; i < span.Length; ++i) {
                span[i].UpdateView(inventoryController.inventory[i]);
            }
        }

        public void RefreshSlot(int index) {
            if (index >= 0 && index < slots.Count)
                slots[index].UpdateView(inventoryController.inventory[index]);
        }

        public void HighlightAt(int selectedIndex) {
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
            inventoryController.ExchangeItem(from, to);
        }

        private void Awake() {
            GetComponentsInChildren(true, slots);
            for (var i = 0; i < slots.Count; i++) {
                var slot = slots[i];
                slot.controller = this;
                slot.index = i;
            }
        }

        private void OnEnable() {
            inventoryRoot.SetActive(true);
            tablePanel.SetActive(false);
            hotBarPanel.SetActive(true);
            RefreshAllSlots();
            HighlightAt(inventoryController.selectedIndex);
            _uiToggle.action.performed += OnUIToggle;
            _uiToggle.action.Enable();
        }

        private void OnDisable() {
            if (tablePanel.activeSelf) {
                tablePanel.SetActive(false);
                CharacterInputManager.Instance.EnableInput();
                CursorManager.Instance.HideCursor();
            }
            inventoryRoot.SetActive(false);
            _uiToggle.action.Disable();
            _uiToggle.action.performed -= OnUIToggle;
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