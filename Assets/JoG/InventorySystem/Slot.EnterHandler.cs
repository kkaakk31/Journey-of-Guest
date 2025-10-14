using UnityEngine.EventSystems;

namespace JoG.InventorySystem {

    public partial class Slot : IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            view.OnPointerEnter(this, eventData);
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData) {
            view.OnPointerMove(this, eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            view.OnPointerExit(this, eventData);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            view.OnBeginDrag(this, eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            view.OnDrag(this, eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            view.OnEndDrag(this, eventData);
        }
    }
}