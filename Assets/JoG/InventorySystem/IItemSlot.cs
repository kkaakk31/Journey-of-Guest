using JoG.Item.Datas;

namespace JoG.InventorySystem {

    public interface IItemSlot {
        ItemData ItemData { get; }
        int ItemCount { get; }
    }
}