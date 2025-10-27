using GuestUnion.Extensions;
using GuestUnion.ObjectPool.Generic;
using JoG.Character;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace JoG {

    [DisallowMultipleComponent]
    public class PlayerCharacterMaster : CharacterMaster {
        private static readonly List<PlayerCharacterMaster> _players = new();
        private readonly NetworkVariable<FixedString32Bytes> _playerName = new(writePerm: NetworkVariableWritePermission.Owner);

        [SerializeField] private GameObject _localPlayerHUDPrefab;
        [SerializeField] private GameObject _remotePlayerHUDPrefab;
        private GameObject _playerHUD;
        public static ReadOnlySpan<PlayerCharacterMaster> Players => _players.AsReadOnlySpan();

        /// <summary>Write: Owner Only.</summary>
        public string PlayerName {
            get => _playerName.Value.ToString();
            set => _playerName.Value = value;
        }

        public event NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate OnPlayerNameChanged {
            add => _playerName.OnValueChanged += value;
            remove => _playerName.OnValueChanged -= value;
        }

        public static bool FindPlayer(string playerName, out PlayerCharacterMaster result) {
            foreach (var player in _players.AsSpan()) {
                if (player._playerName.Value.Equals(playerName)) {
                    result = player;
                    return true;
                }
            }
            result = null;
            return false;
        }

        public void SpawnBody(NetworkObject bodyPrefab, in Vector3 position, in Quaternion rotation) {
            if (AttachedBody != null) {
                Debug.LogWarning("Player already has a body. Cannot spawn another one.");
                return;
            }
            var nob = NetworkManager.SpawnManager.InstantiateAndSpawn(
                bodyPrefab,
                destroyWithScene: true,
                isPlayerObject: true,
                position: position,
                rotation: rotation
            );
            if (nob == null || nob.TrySetParent(NetworkObject, true)) {
                return;
            }
            nob.Despawn();
        }

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            _players.Add(this);
        }

        public override void OnNetworkDespawn() {
            base.OnNetworkDespawn();
            _players.Remove(this);
        }

        protected override void OnBodyAttach(CharacterBody body) {
            body.tag = tag;
            if (_playerHUD == null) {
                if (IsLocalPlayer) {
                    _playerHUD = Instantiate(_localPlayerHUDPrefab, transform);
                } else {
                    _playerHUD = Instantiate(_remotePlayerHUDPrefab, transform);
                }
                using (ListPool<IBodyAttachHandler>.Rent(out var list)) {
                    _playerHUD.GetComponentsInChildren(true, list);
                    foreach (var handler in list) {
                        OnBodyAttached.AddListener(handler.OnBodyAttached);
                        OnBodyDetached.AddListener(handler.OnBodyDetached);
                    }
                }
                using (ListPool<Canvas>.Rent(out var list)) {
                    var uiCamera = Camera.allCameras.Find(c => c.CompareTag("UICamera"));
                    _playerHUD.GetComponentsInChildren(true, list);
                    foreach (var canvas in list) {
                        canvas.worldCamera = uiCamera;
                    }
                }
            }
            _playerHUD.SetActive(true);
        }

        protected override void OnBodyDetach(CharacterBody body) {
            base.OnBodyDetach(body);
            _playerHUD.SetActive(false);
        }
    }
}