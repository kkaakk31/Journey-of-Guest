using EditorAttributes;
using JoG.DebugExtensions;
using JoG.Messages;
using RandomElementsSystem.Types;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Character {

    public class EnemyMaster : CharacterMaster {
        public ushort lifeCount = 1;
        [SerializeField] private NetworkObject _bodyPrefab;
        [SerializeField] private SelectiveRandomWeightTransform _spawnPoints;

        public void SpawnBody(NetworkObject bodyPrefab, in Vector3 position, in Quaternion rotation) {
            if (!HasAuthority) {
                this.LogError("No authority to spawn body.");
                return;
            }
            if (Body != null && Body.IsSpawned) {
                this.LogError("Body is already spawned.");
                return;
            }
            if (bodyPrefab == null) {
                if (_bodyPrefab == null) {
                    this.LogError("Body prefab is not set.");
                    return;
                }
                bodyPrefab = _bodyPrefab;
            }
            var nob = NetworkManager.SpawnManager.InstantiateAndSpawn(
                bodyPrefab,
                destroyWithScene: true,
                isPlayerObject: false,
                position: position,
                rotation: rotation
            );
            if (!nob.TrySetParent(NetworkObject, true)) {
                nob.Despawn();
            }
        }

        public void SpawnBody() {
            var spawnPoint = transform;
            if (_spawnPoints != null) {
                spawnPoint = _spawnPoints.GetRandomValue();
            }
            spawnPoint.GetPositionAndRotation(out var position, out var rotation);
            SpawnBody(_bodyPrefab, position, rotation);
        }

        public override void OnBodyChanged(in CharacterBodyChangedMessage message) {
            base.OnBodyChanged(message);
            if (NetworkManager.ShutdownInProgress) return;
            if (message.changeType is CharacterBodyChangeType.Get) {
                message.body.tag = tag;
            } else if (message.changeType is CharacterBodyChangeType.Lose) {
                if (lifeCount > 0) {
                    if (--lifeCount == 0) {
                        this.Log("Enemy has no more lives. Despawning.");
                    } else {
                        this.Log($"Enemy lost a life. Remaining lives: {lifeCount}");
                        SpawnBody();
                    }
                }
            }
        }

        protected override void OnNetworkSessionSynchronized() {
            if (!HasAuthority) {
                return;
            }
            if (Body != null && Body.IsSpawned) {
                return;
            }
            if (lifeCount == 0) {
                this.LogWarning("Enemy has no lives left. Cannot spawn body.");
                return;
            }
            SpawnBody();
        }
    }
}