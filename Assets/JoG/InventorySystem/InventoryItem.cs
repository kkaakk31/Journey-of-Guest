using UnityEngine;

namespace JoG.InventorySystem {

    public struct InventoryItem {
        public ItemData data;
        public short count;
        public int index;
        public readonly string Name => data?.Name;

        public readonly string Description => data?.Description;

        public readonly Sprite IconSprite => data?.iconSprite;

        public readonly GameObject Prefab => data?.prefab;

        public void Set(ItemData itemData = null, short itemCount = 0) {
            data = itemData;
            count = itemCount;
        }

        public void Exchange(ref InventoryItem other) {
            (other.data, data) = (data, other.data);
            (other.count, count) = (count, other.count);
        }
    }
}