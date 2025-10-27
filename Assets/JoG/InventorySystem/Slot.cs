using EditorAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoG.InventorySystem {

    public partial class Slot : MonoBehaviour {
        [Required] public Image iconImage;
        [Required] public Image slotImage;
        [Required] public TMP_Text countText;

        protected void OnValidate() {
            if (iconImage != null) {
                iconImage.gameObject.SetActive(_itemData is not null);
                iconImage.sprite = _itemData?.iconSprite;
            }
            if (countText != null) {
                countText.gameObject.SetActive(_itemCount > 1);
                countText.text = _itemCount > 1 ? _itemCount.ToString() : string.Empty;
            }
        }
    }
}