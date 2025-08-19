using JoG.Character;
using JoG.UI;
using JoG.UI.Controllers;
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
        public static ReadOnlySpan<PlayerCharacterMaster> Players => _players.AsSpan();
        [field: SerializeField] public CharacterNameplate Nameplate { get; private set; }
        [field: SerializeField] public CharacterViewController ViewController { get; private set; }

        /// <summary>Write: Owner Only.</summary>
        public string PlayerName {
            get => _playerName.Value.ToString();
            set => _playerName.Value = value;
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
            if (Body != null) {
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
            ViewController.enabled = !IsLocalPlayer;
        }

        public override void OnNetworkDespawn() {
            base.OnNetworkDespawn();
            _players.Remove(this);
            ViewController.enabled = false;
        }

        protected void Awake() {
            _playerName.OnValueChanged = (oldValue, newValue) => {
                Nameplate.NameplateText = newValue.ToString();
            };
        }

        protected override void OnNetworkSessionSynchronized() {
            Nameplate.NameplateText = PlayerName;
        }
    }
}