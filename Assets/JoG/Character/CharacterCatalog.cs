using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using YooAsset;

namespace JoG.Character {
    public static class CharacterCatalog {
        private static readonly Dictionary<string, NetworkObject> _nameToCharacters = new();
        private static readonly List<NetworkObject> _playerCharacters = new();

        /// <summary>所有角色（只读）</summary>
        public static IReadOnlyDictionary<string, NetworkObject> AllCharacters => _nameToCharacters;

        /// <summary>所有玩家可选角色（只读）</summary>
        public static IReadOnlyList<NetworkObject> PlayerCharacters => _playerCharacters;

        /// <summary>注册单个角色</summary>
        public static void Register(NetworkObject character, bool isPlayerCharacter = false) {
            if (character == null) {
                Debug.LogError("Attempted to register a null NetworkObject as character.");
                return;
            }
            var nameToken = character.name;
            if (string.IsNullOrEmpty(nameToken)) {
                Debug.LogError("NetworkObject has no name. Cannot register.");
                return;
            }
            _nameToCharacters[nameToken] = character;
            if (isPlayerCharacter && !_playerCharacters.Contains(character)) {
                _playerCharacters.Add(character);
            }
            Debug.Log($"Registered Character: {nameToken}");
        }

        /// <summary>
        /// 批量注册所有角色和玩家可选角色（避免重复注册）
        /// </summary>
        public static void RegisterFromPackage(ResourcePackage package) {
            if (package == null) throw new ArgumentNullException(nameof(package));
            // 1. 注册所有角色
            foreach (var assetInfo in package.GetAssetInfos("character")) {
                var ah = package.LoadAssetSync(assetInfo);
                if (ah.Status == EOperationStatus.Succeed) {
                    if (ah.AssetObject is GameObject go && go.TryGetComponent<NetworkObject>(out var netObj)) {
                        Register(netObj, false);
                    } else {
                        Debug.LogWarning($"[Asset: {ah.AssetObject}] '{assetInfo.AssetPath}' is not a NetworkObject. Skipping registration.");
                    }
                }
                ah.Release();
            }
            // 2. 注册所有玩家可选角色
            foreach (var assetInfo in package.GetAssetInfos("player_character")) {
                var ah = package.LoadAssetSync(assetInfo);
                if (ah.Status == EOperationStatus.Succeed) {
                    if (ah.AssetObject is GameObject go && go.TryGetComponent<NetworkObject>(out var netObj)) {
                        Register(netObj, true);
                    } else {
                        Debug.LogWarning($"[Asset: {ah.AssetObject}] '{assetInfo.AssetPath}' is not a NetworkObject. Skipping registration.");
                    }
                }
                ah.Release();
            }
        }

        /// <summary>查询</summary>
        public static bool TryGetCharacter(string nameToken, out NetworkObject character)
            => _nameToCharacters.TryGetValue(nameToken, out character);

        /// <summary>清空所有已注册</summary>
        public static void Clear() {
            _nameToCharacters.Clear();
            _playerCharacters.Clear();
        }
    }
}