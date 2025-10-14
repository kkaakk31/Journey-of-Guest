using UnityEngine;

namespace JoG.InventorySystem {

    public interface IItemSlot {
        ItemData ItemData { get; set; }
        short Count { get; set; }
        string Name { get; }
        string Description { get; }
        Sprite IconSprite { get; }
        GameObject Prefab { get; }
        int Index { get; set; }

        void Set(ItemData itemData = null, short itemCount = 0);

        void Exchange(in IItemSlot other);
    }
}