using EditorAttributes;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Projectiles {

    public class SpawnChildrenOnCollision : MonoBehaviour {
        [Required] public ProjectileDamageData damageData;
        [Min(1)] public int spawnCount = 1;
        public NetworkObject childPrefab;
        public ESpawnForwardMode spawnForwardMode;

        public void Handle(CollisionMessage message) {
            var rotation = spawnForwardMode switch {
                ESpawnForwardMode.CollisionNormal => Quaternion.LookRotation(message.normal, Vector3.up),
                _ => transform.rotation,
            };
            for (int i = 0; i < spawnCount; i++) {
                FireChild(message.position, rotation);
            }
        }

        public void FireChild(in Vector3 position, in Quaternion rotation) {
            var nob = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(childPrefab, position: position, rotation: rotation);
            nob.GetComponent<ProjectileDamageData>().Initialize(damageData);
        }

        public enum ESpawnForwardMode {
            ParentForward,
            CollisionNormal
        }
    }
}