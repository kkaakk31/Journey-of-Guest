using JoG.Character.InputBanks;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Character {

    [DisallowMultipleComponent]
    public partial class CharacterBody : NetworkBehaviour {
        private static readonly List<CharacterBody> _characters = new();
        private readonly Dictionary<string, InputBank> _nameToInputBank = new();
        [SerializeField] private CharacterModel _model;
        [SerializeField] private Transform _aimOriginTransform;
        private CharacterMaster _master;
        public static ReadOnlySpan<CharacterBody> Characters => _characters.AsSpan();
        public CharacterMaster Master => _master;
        public Animator Animator => _model.Animator;

        public Transform AimOriginTransform => _aimOriginTransform;

        public Vector3 Center => _model.Center.position;

        public Vector3 Top => _model.Top.position;

        public Vector3 Bottom => _model.Bottom.position;

        public Vector3 AimOrigin => _aimOriginTransform.position;

        public float Radius => _model.Radius;

        public float Height => _model.Height;

        public CharacterModel Model => _model;

        public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject) {
            if (_master != null) {
                _master.DetachBody(this);
            }
            if (parentNetworkObject is null) {
                _master = null;
                return;
            }
            if (parentNetworkObject.TryGetComponent(out _master)) {
                _master.AttachBody(this);
            }
        }

        public override void OnNetworkSpawn() {
            if (IsOwner) {
                MaxHP = (uint)Math.Round(_maxHPBase * _maxHPCoefficient);
                HP = MaxHP;
            }
            InitializeBuff();
        }

        public override void OnNetworkDespawn() {
            DeinitializeBuff();
            if (_master != null) {
                _master.DetachBody(this);
                _master = null;
            }
        }

        public T GetInputBank<T>(string name) where T : InputBank, new() {
            //ref var inputBankRef = ref _nameToInputBank.InternalGetValueRefOrAddDefault(name, out var exists);
            //if (exists) {
            //    return inputBankRef as T;
            //} else {
            //    var inputBank = new T();
            //    inputBankRef = inputBank;
            //    return inputBank;
            //}
            if (_nameToInputBank.TryGetValue(name, out var inputBank)) {
                return inputBank as T;
            } else {
                inputBank = new T();
                _nameToInputBank[name] = inputBank;
                return inputBank as T;
            }
        }
    }
}