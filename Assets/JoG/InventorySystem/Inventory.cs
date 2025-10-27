using GuestUnion.Extensions;
using JoG.Item.Datas;
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace JoG.InventorySystem {

    [Serializable]
    public class Inventory : MonoBehaviour, IInventory {
        private IItemSlot[] _items;
        public int Size => _items.Length;
        public ReadOnlySpan<IItemSlot> Items => new(_items);
        public IItemSlot this[int index] => _items[index];

        public bool TryGetItem(int index, out IItemSlot item) => _items.TryGet(index, out item);

        public int GetItemCount(int index) {
            return _items[index].ItemCount;
        }

        public int GetItemCount(ItemData itemData) {
            var count = 0;
            foreach (var item in Items) {
                if (item.ItemData == itemData) {
                    count += item.ItemCount;
                }
            }
            return count;
        }

        public void ExchangeItem(int fromIndex, int toIndex) {
            if (fromIndex != toIndex) {
                _items[fromIndex].Exchange(_items[toIndex]);
            }
        }

        public void SetItemCount(int index, int count) {
            _items[index].ItemCount = count;
        }

        /// <returns>未成功移除的物品数量</returns>
        public int RemoveItem(int index, int amount) {
            if (amount <= 0) {
                return 0;
            }
            var item = _items[index];
            if (item.ItemData is not null) {
                var toRemove = Math.Min(amount, item.ItemCount);
                item.ItemCount -= amount;
                amount -= toRemove;
            }
            return amount;
        }

        /// <returns>未成功添加到背包的物品数量</returns>
        public int AddItem(int index, int amount) {
            if (amount <= 0) {
                return 0;
            }
            var item = _items[index];
            if (item.ItemData is not null) {
                var remainingCount = item.ItemData.maxStack - item.ItemCount;
                var toAdd = Math.Min(amount, remainingCount);
                item.ItemCount += amount;
                amount -= toAdd;
            }
            return amount;
        }

        /// <summary>添加一定数量的物品到背包中。</summary>
        /// <param name="itemData">要添加的物品</param>
        /// <param name="amount">要添加的数量</param>
        /// <returns>未成功添加到背包的物品数量</returns>
        public int AddItem(ItemData itemData, int amount = 1) {
            if (itemData is null) {
                return amount;
            }
            if (amount <= 0) {
                return 0;
            }
            var span = new Span<IItemSlot>(_items);
            foreach (var item in span) {
                if (item.ItemData == itemData) {
                    var remainingCount = item.ItemData.maxStack - item.ItemCount;
                    var toAdd = Math.Min(amount, remainingCount);
                    item.ItemCount += toAdd;
                    amount -= toAdd;
                    if (amount <= 0) {
                        return 0;
                    }
                }
            }
            foreach (var item in span) {
                if (item.ItemData is null) {
                    var toAdd = Math.Min(amount, itemData.maxStack);
                    item.ItemData = itemData;
                    item.ItemCount = toAdd;
                    amount -= toAdd;
                    if (amount <= 0) {
                        return 0;
                    }
                }
            }
            return amount;
        }

        /// <summary>移除背包中指定物品。</summary>
        /// <param name="itemData">要移除的物品</param>
        /// <param name="amount">要移除的数量</param>
        /// <returns>未成功移除的物品数量</returns>
        public int RemoveItem(ItemData itemData, int amount) {
            if (itemData is null) {
                return amount;
            }
            if (amount <= 0) {
                return 0;
            }
            foreach (ref var item in _items.AsSpan()) {
                if (item.ItemData == itemData) {
                    var toRemove = Math.Min(amount, item.ItemCount);
                    item.ItemCount -= toRemove;
                    amount -= toRemove;
                    if (amount <= 0) {
                        return 0;
                    }
                }
            }
            return amount;
        }

        public void FromJson(string json) {
            if (json.IsNullOrEmpty()) {
                return;
            }
            var serializedItems = JsonConvert.DeserializeObject<SerializedItem[]>(json);
            if (serializedItems.IsNullOrEmpty()) {
                return;
            }
            foreach (var item in new ReadOnlySpan<SerializedItem>(serializedItems)) {
                if (item.index < 0 || item.index >= _items.Length) {
                    continue;
                }
                if (ItemCatalog.TryGetItemDef(item.itemToken, out var itemData)) {
                    _items[item.index].ItemData = itemData;
                    _items[item.index].ItemCount = item.itemCount;
                }
            }
        }

        public string ToJson() {
            var itemDataList = new SerializedItem[_items.Length];
            foreach (var item in new ReadOnlySpan<IItemSlot>(_items)) {
                if (item.ItemData is not null) {
                    itemDataList[item.Index] = new SerializedItem {
                        itemToken = item.ItemData.nameToken,
                        itemCount = item.ItemCount,
                        index = item.Index
                    };
                }
            }
            return JsonConvert.SerializeObject(itemDataList);
        }

        protected void Awake() {
            _items = GetComponentsInChildren<IItemSlot>();
            for (var i = 0; i < _items.Length; ++i) {
                _items[i].Index = i;
            }
        }

        [Serializable]
        private struct SerializedItem {
            public string itemToken;
            public int itemCount;
            public int index;
        }
    }
}