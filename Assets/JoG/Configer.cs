using Hjson;
using JoG.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace JoG {

    public static class Configer {
        private static readonly Dictionary<string, object> _config = new();

        public static void Initialize() {
            Load(Path.Combine(Application.streamingAssetsPath, "config.hjson"));
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Inject(assembly);
            }
        }

        public static void Load(string path) {
            foreach (var kv in HjsonLoader.LoadHjsonAsDictionary(path)) {
                switch (kv.Value.JsonType) {
                    case JsonType.String:
                    case JsonType.Number:
                    case JsonType.Boolean:
                        try {
                            var value = kv.Value.ToValue();
                            _config[kv.Key] = value;
                            Debug.Log($"[ConfigManager] Loaded: '{kv.Key}' = {value}");
                        } catch (Exception ex) {
                            Debug.LogException(ex);
                        }
                        break;
                }
            }
        }

        public static object GetConfig(string key) {
            if (key is not null && _config.TryGetValue(key, out var value)) {
                return value;
            }
            return null;
        }

        public static bool TryGetConfig(string key, out object value) {
            if (key is not null && _config.TryGetValue(key, out var obj)) {
                value = obj;
                return true;
            }
            value = default;
            return false;
        }

        public static void Inject(Assembly assembly) {
            foreach (var type in assembly.GetTypes()) {
                Inject(type);
            }
        }

        public static void Inject(Type type) {
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).AsSpan()) {
                var attr = field.GetCustomAttribute<FromConfigAttribute>();
                if (attr is null) continue;
                if (!_config.TryGetValue(attr.Key, out var value)) {
                    Debug.LogWarning($"[ConfigManager] Config key '{attr.Key}' not found for {type.FullName}.{field.Name}");
                    continue;
                }
                try {
                    var converted = Convert.ChangeType(value, field.FieldType);
                    field.SetValue(null, converted);
                    Debug.Log($"[ConfigManager] Injected: {type.FullName}.{field.Name} <= {converted}");
                } catch (Exception ex) {
                    Debug.LogException(ex);
                }
            }
        }
    }
}