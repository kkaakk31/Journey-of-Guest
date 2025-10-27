using GuestUnion.TooltipSystem.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace JoG.InventorySystem {

    public partial class Slot : IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [Inject] private TooltipView tooltipView;
        [Inject] private DraggingItemView draggingItemView;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            if (IsEmpty || eventData.dragging) return;
            tooltipView.Clear();
            _itemData.BuildTooltip(tooltipView);
            tooltipView.PlaceNearPoint(eventData.pointerCurrentRaycast.screenPosition);
            tooltipView.Show(.5f);
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData) {
            //view.OnPointerMove(this, eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            if (IsEmpty || eventData.dragging) return;
            tooltipView.Hide(0);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            if (IsEmpty) return;
            tooltipView.Hide(0);
            draggingItemView.RefreshView(iconImage.sprite, countText.text);
            draggingItemView.Show();
            iconImage.gameObject.SetActive(false);
            countText.gameObject.SetActive(false);
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            if (IsEmpty) return;
            if (!eventData.dragging) {
                Debug.Log("no dragging");
            }
            draggingItemView.Position = eventData.pointerCurrentRaycast.worldPosition;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            if (IsEmpty) return;
            if (eventData.pointerEnter is not null && eventData.pointerEnter.TryGetComponent<Slot>(out var otherSlot)) {
                Exchange(otherSlot);
            } else {
                iconImage.gameObject.SetActive(true);
                countText.gameObject.SetActive(true);
            }
            draggingItemView.Hide();
        }
    }
}