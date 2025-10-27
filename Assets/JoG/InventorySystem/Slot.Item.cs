using GuestUnion.Extensions;
using JoG.Item.Datas;

namespace JoG.InventorySystem {

    public partial class Slot : IItemSlot {
        private ItemData _itemData;
        private int _itemCount;

        public ItemData ItemData {
            get => _itemData;
            set {
                _itemData = value;
                if (value == null) {
                    _itemCount = 0;
                    countText.gameObject.SetActive(false);
                    iconImage.gameObject.SetActive(false);
                } else {
                    iconImage.gameObject.SetActive(true);
                    iconImage.sprite = value.iconSprite;
                }
            }
        }

        public int ItemCount {
            get => _itemCount;
            set {
                if (_itemData == null) {
                    return;
                }
                _itemCount = value.Clamp(0, _itemData.maxStack);
                if (_itemCount > 1) {
                    countText.gameObject.SetActive(true);
                    countText.text = _itemCount.ToString();
                    return;
                }
                countText.gameObject.SetActive(false);
                if (_itemCount == 0) {
                    _itemData = null;
                    iconImage.gameObject.SetActive(false);
                }
            }
        }
        public bool IsEmpty => _itemCount is 0 || _itemData == null;

        public int Index { get; set; }

        public void Exchange(IItemSlot other) {
            var tmp = ItemData;
            var tmp2 = ItemCount;
            ItemData = other.ItemData;
            ItemCount = other.ItemCount;
            other.ItemData = tmp;
            other.ItemCount = tmp2;
        }
    }
}