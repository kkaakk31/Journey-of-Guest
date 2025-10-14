using System.Collections.Generic;

namespace JoG.InventorySystem {

    public interface IInventory<TItem> where TItem : IItemSlot {
        public ICollection<TItem> Items { get; }
    }
}