using Hjson;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace JoG.Localization {

    public static class Localizer {
        private static Dictionary<string, string> _table = new();
        private static string _currentLanguage = "zh-CN";

        public static string CurrentLanguage {
            get => _currentLanguage;
            set {
                if (_currentLanguage == value) return;
                _currentLanguage = value;
                Load(Path.Combine(Application.streamingAssetsPath, $"Localization/{_currentLanguage}.hjson"));
            }
        }

        public static void Initialize() {
            Load(Path.Combine(Application.streamingAssetsPath, "Localization/zh-CN.hjson"));
        }

        public static string GetString(string key) {
            return key is null
                ? string.Empty
                : (_table.TryGetValue(key, out var value) ? value : key);
        }

        public static string GetString(string key, params object[] args) {
            return string.Format(GetString(key), args);
        }

        public static string GetString(string token, object arg0) {
            return string.Format(GetString(token), arg0);
        }

        public static string GetString(string token, object arg0, object arg1) {
            return string.Format(GetString(token), arg0, arg1);
        }

        public static string GetString(string token, object arg0, object arg1, object arg2) {
            return string.Format(GetString(token), arg0, arg1, arg2);
        }

        public static void Load(string path) {
            var table = HjsonLoader.LoadHjsonAsDictionary(path);
            if (table is null) return;
            // 将table中的键值对转换为字符串并存储到_table中，同时将字符串中的占位符替换为config中的对应值
            foreach (var kv in table) {
                if (kv.Value.JsonType is not JsonType.String) {
                    continue;
                }
                var value = kv.Value.Qstr();
                // 获取当前key的路径（如 buff.bleeding.desc -> buff.bleeding）
                var keyPath = kv.Key;
                var lastDotIndex = keyPath.LastIndexOf('.');
                var parentPath = lastDotIndex > 0 ? keyPath[..lastDotIndex] : keyPath;

                // 查找该路径下所有config占位符 例如 {tickInterval}
                value = Regex.Replace(
                    value,
                    @"\{([^\{\}]+)\}",
                    match => {
                        var placeholder = match.Groups[1].Value;
                        // 先尝试在同路径下查找
                        var configKey = $"{parentPath}.{placeholder}";
                        if (Configer.TryGetConfig(configKey, out var configValue)) {
                            return configValue.ToString();
                        }
                        // 再尝试全局查找
                        if (Configer.TryGetConfig(placeholder, out configValue)) {
                            return configValue.ToString();
                        }
                        // 找不到就原样返回
                        return match.Value;
                    }
                );
                _table[kv.Key] = value;
                Debug.Log($"[LocalizationManager] Loaded: '{kv.Key}' = {value}");
            }
        }
    }
}