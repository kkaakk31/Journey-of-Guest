using EditorAttributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JoG.InventorySystem {

    public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [NonSerialized] public InventoryView view;
        [NonSerialized] public int index;
        [Required] public GameObject iconGameObject;
        [Required] public Image iconImage;
        [Required] public Image slotImage;
        [Required] public TMP_Text countText;

        public void UpdateView(in InventoryItem inventoryItem) {
            if (inventoryItem.count <= 0) {
                iconGameObject.SetActive(false);
                return;
            }
            iconGameObject.SetActive(true);
            iconImage.sprite = inventoryItem.IconSprite;
            countText.text = inventoryItem.count > 1 ? inventoryItem.count.ToString() : string.Empty;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData) {
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            if (iconGameObject.activeSelf) {
                iconGameObject.SetActive(false);
                view.ShowDragItem(iconImage.sprite, countText.text);
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            if (eventData.dragging) {
                view.SetDragItemPosition(eventData.pointerCurrentRaycast.worldPosition);
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            if (eventData.pointerEnter is not null && eventData.pointerEnter.CompareTag("Slot")) {
                var otherSlot = eventData.pointerEnter.GetComponent<Slot>();
                view.ExchangeItem(index, otherSlot.index);
            } else {
                iconGameObject.SetActive(true);
            }
            view.HideDragItem();
        }
    }
}