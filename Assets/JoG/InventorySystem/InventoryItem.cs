using JoG.Localization;
using UnityEngine;

namespace JoG.InventorySystem {

    public class InventoryItem {
        private Inventory _inventory;
        private ItemData _data;
        private byte _count;
        private int _index;
        public int Index { get => _index; internal set => _index = value; }
        public string Name => LocalizationManager.GetString(_data?.nameToken);

        public string Description => LocalizationManager.GetString(_data?.descriptionToken);

        public Sprite Icon => _data?.icon;

        public GameObject Prefab => _data?.prefab;
        public Inventory Inventory => _inventory;

        public byte Count {
            get => _count;
            set {
                if (_count == value) return;
                _count = value;
                if (value == 0) {
                    _data = null;
                }
                _inventory.PublishItemChanged(_index);
            }
        }

        public ItemData Data {
            get => _data;
            set {
                if (_data == value) return;
                _data = value;
                if (value == null) {
                    _count = 0;
                }
                _inventory.PublishItemChanged(_index);
            }
        }

        public InventoryItem(Inventory inventory, int index) {
            _inventory = inventory;
            _index = index;
        }

        public void SetDataAndCount(ItemData itemData = null, byte itemCount = 0) {
            if (_data == itemData && _count == itemCount) return;
            _data = itemData;
            _count = itemCount;
            _inventory.PublishItemChanged(_index);
        }
    }
}