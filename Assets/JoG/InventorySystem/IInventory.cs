using JoG.Item.Datas;
using System;

namespace JoG.InventorySystem {

    public interface IInventory {
        IItemSlot this[int index] { get; }
        public ReadOnlySpan<IItemSlot> Items { get; }
        int Size { get; }
        int GetItemCount(ItemData itemData);
        int AddItem(ItemData itemData, int amount = 1);
        int RemoveItem(ItemData itemData, int amount);
        int GetItemCount(int index);
        int AddItem(int index, int amount);
        int RemoveItem(int index, int amount);
        void SetItemCount(int index, int count);
        void ExchangeItem(int fromIndex, int toIndex);
    }
}