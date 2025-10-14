using EditorAttributes;
using GuestUnion;
using JoG.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace JoG.InventorySystem {

    public class InventoryView : MonoBehaviour {
        [Required] public InventoryController controller;
        [Required] public GameObject inventoryRoot;
        [Required] public GameObject tablePanel;
        [Required] public GameObject hotBarPanel;
        public DragItem dragItem;
        public Color highlightColor = Color.yellow;
        public Color normalColor = Color.white;
        public TooltipView tooltipView;
        [SerializeField, Required] private InputActionReference _tableToggle;
        private List<Slot> slots = new();
        private Slot highlightSlot;

        public void RefreshAllSlots() {
            foreach (var slot in slots.AsSpan()) {
                //slot.UpdateView(controller.inventory[slot.Index]);
            }
        }

        public void RefreshSlot(int index) {
            if (index >= 0 && index < slots.Count) {
                //slots[index].UpdateView(controller.inventory[index]);
            }
        }

        public void HighlightSlot(int index) {
            if (highlightSlot != null) {
                highlightSlot.slotImage.color = normalColor;
            }
            if (slots.TryGetAt(index, out highlightSlot)) {
                highlightSlot.slotImage.color = highlightColor;
            }
        }

        public void OnPointerEnter(Slot slot, PointerEventData eventData) {
            var item = controller.inventory[slot.Index];
            if (item.count > 0) {
                tooltipView.SetTooltip(item.Name);
                tooltipView.SetPosition(eventData.pointerCurrentRaycast.worldPosition);
                tooltipView.Show();
            }
        }

        public void OnPointerMove(Slot slot, PointerEventData eventData) {
            if (!eventData.dragging) {
                tooltipView.SetPosition(eventData.pointerCurrentRaycast.worldPosition);
            }
        }

        public void OnPointerExit(Slot slot, PointerEventData eventData) {
            tooltipView.Hide();
        }

        public void OnBeginDrag(Slot slot, PointerEventData eventData) {
            if (slot.iconObject.activeSelf) {
                slot.iconObject.SetActive(false);
                dragItem.iconImage.sprite = slot.iconImage.sprite;
                dragItem.countText.text = slot.countText.text;
                dragItem.gameObject.SetActive(true);
                tooltipView.Hide();
            }
        }

        public void OnDrag(Slot slot, PointerEventData eventData) {
            if (eventData.dragging && dragItem.gameObject.activeSelf) {
                dragItem.transform.localPosition = eventData.pointerCurrentRaycast.worldPosition;
            }
        }

        public void OnEndDrag(Slot slot, PointerEventData eventData) {
            if (!dragItem.gameObject.activeSelf) return;
            if (eventData.pointerEnter is not null && eventData.pointerEnter.TryGetComponent<Slot>(out var otherSlot)) {
                controller.ExchangeItem(slot.Index, otherSlot.Index);
            } else {
                slot.iconObject.SetActive(true);
            }
            dragItem.gameObject.SetActive(false);
        }

        private void Awake() {
            GetComponentsInChildren(true, slots);
            for (var i = 0; i < slots.Count; i++) {
                var slot = slots[i];
                slot.view = this;
                slot.Index = i;
            }
        }

        private void OnEnable() {
            inventoryRoot.SetActive(true);
            tablePanel.SetActive(false);
            hotBarPanel.SetActive(true);
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