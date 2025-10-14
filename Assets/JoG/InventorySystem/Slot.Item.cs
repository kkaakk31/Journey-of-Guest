using UnityEngine;

namespace JoG.InventorySystem {

    public partial class Slot : IItemSlot {
        private ItemData _itemData;
        private short _itemCount;

        public ItemData ItemData {
            get => _itemData;
            set {
                _itemData = value;
                iconImage.sprite = value == null ? null : value.iconSprite;
            }
        }

        public short Count {
            get => _itemCount;
            set {
                _itemCount = value;
                countText.text = value > 1 ? value.ToString() : string.Empty;
                iconObject.SetActive(value > 0);
            }
        }

        public string Name => _itemData?.Name;

        public string Description => _itemData?.Description;

        public Sprite IconSprite => _itemData?.iconSprite;

        public GameObject Prefab => _itemData?.prefab;

        public int Index { get; set; }

        public void Set(ItemData itemData = null, short itemCount = 0) {
            ItemData = itemData;
            Count = itemCount;
        }

        public void Exchange(in IItemSlot other) {
            (other.ItemData, ItemData) = (ItemData, other.ItemData);
            (other.Count, Count) = (Count, other.Count);
        }
    }
}