using JoG.Item.Datas;

namespace JoG.InventorySystem {

    public interface IItemSlot {
        ItemData ItemData { get; set; }
        int ItemCount { get; set; }
        bool IsEmpty { get; }
        int Index { get; set; }
        void Exchange(IItemSlot other);
    }
}