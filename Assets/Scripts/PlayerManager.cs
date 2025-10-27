using ANU.IngameDebug.Console;
using GuestUnion.ObjectPool.Generic;
using GuestUnion.YooAsset;
using JoG.Character;
using JoG.Networking;
using System;
using System.Runtime.InteropServices;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using VContainer;

namespace JoG {

    [DebugCommandPrefix("player")]
    public class PlayerManager : NetworkBehaviour {
        public YooAssetReference<GameObject> masterPrefab;
        public YooAssetReference<GameObject> bodyPrefab;
        [Inject] private IAuthenticationService _authenticationService;
        [Inject] private ISessionService _sessionService;
        private PlayerCharacterMaster _master;
        [SerializeField] private GameObject[] _bodyAttachHandlerObjects = Array.Empty<GameObject>();

        [DebugCommand(Description = "Spawn a default body at a random spawn point")]
        public void SpawnBody() {
            if (_master == null) return;
            var bodyNO = bodyPrefab.AssetObject.GetComponent<NetworkObject>();
            transform.GetPositionAndRotation(out var position, out var rotation);
            _master.SpawnBody(bodyNO, position, rotation);
        }

        public override void OnDestroy() {
            base.OnDestroy();
            masterPrefab.Dispose();
            bodyPrefab.Dispose();
        }

        protected void Awake() {
            masterPrefab.LoadAssetSync();
            bodyPrefab.LoadAssetSync();
        }

        protected override void OnNetworkSessionSynchronized() {
            var networkPrefab = masterPrefab.AssetObject.GetComponent<NetworkObject>();
            var no = NetworkManager.SpawnManager.InstantiateAndSpawn(networkPrefab, destroyWithScene: true, isPlayerObject: true);
            _master = no.GetComponent<PlayerCharacterMaster>();
            _master.PlayerName = _authenticationService.PlayerName;
            using (ListPool<IBodyAttachHandler>.Rent(out var list)) {
                foreach (var gameObject in new ReadOnlySpan<GameObject>(_bodyAttachHandlerObjects)) {
                    gameObject.GetComponentsInChildren(true, list);
                    foreach (var handler in list.AsReadOnlySpan()) {
                        _master.OnBodyAttached.AddListener(handler.OnBodyAttached);
                        _master.OnBodyDetached.AddListener(handler.OnBodyDetached);
                    }
                }
            }
            SpawnBody();
        }
    }
}