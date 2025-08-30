using GuestUnion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace JoG.InventorySystem {

    [Serializable]
    public class Inventory {
        private InventoryItem[] _items;
        public int Size => _items.Length;
        public ReadOnlySpan<InventoryItem> Items => new(_items);

        public Inventory(int size) {
            _items = new InventoryItem[size];
            for (var i = 0; i < size; ++i) {
                _items[i].index = i;
            }
        }

        public ref readonly InventoryItem this[int index] => ref _items[index];

        public bool TryGetItem(int index, out InventoryItem item) => _items.TryGet(index, out item);

        public short GetItemCount(int index) {
            return _items[index].count;
        }

        public void ExchangeItem(int fromIndex, int toIndex) {
            if (fromIndex != toIndex) {
                _items[fromIndex].Exchange(ref _items[toIndex]);
            }
        }

        public void SetItemCount(int index, short count) {
            if (count > 0) {
                _items[index].count = count;
            } else {
                _items[index].Set();
            }
        }

        public void RemoveItem(int index, short count) {
            if (count <= 0) return;
            ref var item = ref _items[index];
            if (item.count > count) {
                item.count -= count;
            } else {
                item.Set();
            }
        }

        public void AddItem(int index, short count) {
            if (count <= 0) return;
            ref var item = ref _items[index];
            if (item.data != null) {
                item.count += count;
            }
        }

        /// <summary>添加物品到背包中，如果背包中已存在该物品，则增加数量；如果背包中没有空位，则不添加。</summary>
        /// <param name="itemData">要添加的物品</param>
        /// <param name="count">要添加的数量</param>
        /// <returns>添加物品在背包中的索引，如果背包已满或者添加数量为0或者物品数据为null，则返回-1</returns>
        public int AddItem(ItemData itemData, short count) {
            if (itemData is null || count <= 0) {
                return -1;
            }
            var span = new Span<InventoryItem>(_items);
            var firstEmptyIndex = -1;
            foreach (ref var item in span) {
                if (item.data == itemData) {
                    item.count += count;
                    return item.index;
                }
                if (firstEmptyIndex == -1 && item.data is null) {
                    firstEmptyIndex = item.index;
                }
            }
            if (firstEmptyIndex != -1) {
                span[firstEmptyIndex].Set(itemData, count);
            }
            return firstEmptyIndex;
        }

        /// <summary>移除指定物品的数量，如果数量大于等于当前物品数量，则将该物品从背包中移除。</summary>
        /// <param name="itemData">指定的物品</param>
        /// <param name="count">要移除的数量</param>
        /// <returns>移除物品在背包中的索引，未找到或者添加数量为0或者物品数据为null，则返回-1</returns>
        public int RemoveItem(ItemData itemData, short count) {
            if (itemData is null || count <= 0) {
                return -1;
            }
            foreach (ref var item in _items.AsSpan()) {
                if (item.data == itemData) {
                    if (item.count > count) {
                        item.count -= count;
                    } else {
                        item.Set();
                    }
                    return item.index;
                }
            }
            return -1;
        }

        public void FromJson(string json) {
            if (json.IsNullOrEmpty()) {
                return;
            }
            var serializedItems = JsonConvert.DeserializeObject<List<SerializedItem>>(json);
            if (serializedItems.IsNullOrEmpty()) {
                return;
            }
            foreach (ref var item in serializedItems.AsSpan()) {
                if (item.index < 0 || item.index >= _items.Length) {
                    continue;
                }
                if (ItemCatalog.TryGetItemDef(item.itemToken, out var itemData)) {
                    _items[item.index].Set(itemData, item.itemCount);
                }
            }
        }

        public string ToJson() {
            var itemDataList = new List<SerializedItem>();
            foreach (ref readonly var item in new ReadOnlySpan<InventoryItem>(_items)) {
                if (item.count > 0 && item.data != null) {
                    itemDataList.Add(new SerializedItem {
                        itemToken = item.data.nameToken,
                        itemCount = item.count,
                        index = item.index
                    });
                }
            }
            return JsonConvert.SerializeObject(itemDataList);
        }

        [Serializable]
        private struct SerializedItem {
            public string itemToken;
            public short itemCount;
            public int index;
        }
    }
}