using ANU.IngameDebug.Console;
using Cysharp.Threading.Tasks;
using GuestUnion.Extensions;
using JoG.Item.Datas;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using YooAsset;

namespace JoG.InventorySystem {

    [DebugCommandPrefix("item")]
    public static class ItemCatalog {
        private static readonly Dictionary<string, ItemData> _nameToItemDatas = new();
        public static IReadOnlyDictionary<string, ItemData> AllItems => _nameToItemDatas;

        /// <summary>单个注册ItemData</summary>
        public static void Register(ItemData item) {
            if (item == null) {
                Debug.LogError("Attempted to register a null ItemData.");
                return;
            }
            if (item.nameToken.IsNullOrEmpty()) {
                Debug.LogError($"ItemData '{item.name}' has no nameToken set. Cannot register.");
                return;
            }
            if (_nameToItemDatas.TryAdd(item.nameToken, item)) {
                Debug.Log($"Registered ItemData: {item.nameToken}");
            } else {
                Debug.LogWarning($"ItemData '{item.nameToken}' is already registered. Overwriting existing entry.");
            }
        }

        public static void Unregister(ItemData itemData) {
            if (itemData == null) {
                Debug.LogError("Attempted to unregister a null ItemData.");
                return;
            }
            Unregister(itemData.nameToken);
        }

        [DebugCommand("unregister", "Unregister an ItemData by its nameToken.")]
        public static void Unregister(string itemToken) {
            if (itemToken.IsNullOrEmpty()) {
                Debug.LogError("Attempted to unregister an ItemData with a null or empty nameToken.");
                return;
            }
            if (_nameToItemDatas.Remove(itemToken)) {
                Debug.Log($"Unregistered ItemData: {itemToken}");
            } else {
                Debug.LogWarning($"No ItemData found with nameToken '{itemToken}' to unregister.");
            }
        }

        [DebugCommand("unregister_unloaded", "Unregister all ItemData entries that are null (unloaded).")]
        public static void UnregisterUnloadedItems() {
            var keysToRemove = new List<string>();
            foreach (var kvp in _nameToItemDatas) {
                if (kvp.Value == null) {
                    keysToRemove.Add(kvp.Key);
                }
            }
            foreach (var key in keysToRemove) {
                _nameToItemDatas.Remove(key);
            }
            Debug.Log($"Unregistered invalid ItemTokens: {string.Join(';', keysToRemove)}");
        }

        /// <summary>批量注册：通过YooAsset资源包和标签加载所有ItemData</summary>
        public static async UniTask RegisterFromPackageAsync(ResourcePackage package) {
            if (package == null) throw new ArgumentNullException(nameof(package));
            foreach (var assetInfo in package.GetAssetInfos("item")) {
                var ah = package.LoadAssetAsync(assetInfo);
                await ah.Task;
                if (ah.Status == EOperationStatus.Succeed) {
                    if (ah.AssetObject is ItemData itemData) {
                        Register(itemData);
                    } else {
                        Debug.LogWarning($"[Asset: {ah.AssetObject}] '{assetInfo.AssetPath}' is not of type ItemData. Skipping registration.");
                    }
                }
            }
        }

        /// <summary>查询</summary>
        public static bool TryGetItemDef(string itemToken, out ItemData itemData) {
            if (itemToken.IsNullOrEmpty()) {
                itemData = null;
                return false;
            }
            return _nameToItemDatas.TryGetValue(itemToken, out itemData);
        }

        /// <summary>清空所有已注册</summary>
        public static void Clear() => _nameToItemDatas.Clear();

        [DebugCommand("print_all", "Print all registered items in the ItemCatalog.")]
        public static void PrintAllRegisteredItems() {
            var sb = new StringBuilder("Registered Item: \n");
            var count = 0;
            foreach (var itemData in _nameToItemDatas.Values) {
                count++;
                sb.AppendFormat($"[{0}]Token: {1}, Name: {2}\n", count, itemData.nameToken, itemData.Name);
            }
            Debug.Log(sb);
        }
    }
}