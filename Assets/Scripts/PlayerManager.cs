using ANU.IngameDebug.Console;
using RandomElementsSystem.Types;
using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace JoG {

    [DebugCommandPrefix("player")]
    public class PlayerManager : NetworkBehaviour {
        public NetworkObject networkPlayerPrefab;
        public NetworkObject defaultBodyPrefab;
        [Inject] private IAuthenticationService _authenticationService;
        private PlayerCharacterMaster _master;
        [SerializeField] private GameObject[] _autoInjectObjects = Array.Empty<GameObject>();
        [SerializeField] private SelectiveRandomWeightTransform _spawnPoints;

        [DebugCommand(Description = "Spawn a default body at a random spawn point")]
        public void SpawnBody() {
            if (_master == null) return;
            var spanwPoint = _spawnPoints.GetRandomValue();
            spanwPoint.GetPositionAndRotation(out var position, out var rotation);
            _master.SpawnBody(defaultBodyPrefab, position, rotation);
        }

        protected override void OnNetworkSessionSynchronized() {
            var playerName = _authenticationService.PlayerName;
            if (PlayerCharacterMaster.FindPlayer(playerName, out _master)) {
                _master.NetworkObject.ChangeOwnership(NetworkManager.LocalClientId);
            } else {
                _master = NetworkManager.SpawnManager.InstantiateAndSpawn(
                    networkPlayerPrefab,
                    NetworkManager.LocalClientId,
                    false,
                    true).GetComponent<PlayerCharacterMaster>();
                _master.PlayerName = playerName;
            }
            var resolver = _master.GetComponent<LifetimeScope>().Container;
            foreach (var obj in new ReadOnlySpan<GameObject>(_autoInjectObjects)) {
                resolver.InjectGameObject(obj);
            }
            SpawnBody();
        }
    }
}