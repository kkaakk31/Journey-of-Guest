using EditorAttributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JoG.InventorySystem {

    public partial class Slot : MonoBehaviour {
        [NonSerialized] public InventoryView view;
        [Required] public GameObject iconObject;
        [Required] public Image iconImage;
        [Required] public Image slotImage;
        [Required] public TMP_Text countText;

        protected void Awake() {
            view = GetComponentInParent<InventoryView>();
        }

        protected void OnValidate() {
            if (_itemData == null || _itemCount <= 0) {
                iconObject.SetActive(false);
            } else {
                iconObject.SetActive(true);
                if (iconImage != null) {
                    iconImage.sprite = _itemData.iconSprite;
                }
                if (countText != null) {
                    countText.text = _itemCount > 1 ? _itemCount.ToString() : string.Empty;
                }
            }
        }
    }
}