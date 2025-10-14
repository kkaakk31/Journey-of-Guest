using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Projectiles {

    public class SpawnChildrenOnCollision : MonoBehaviour {
        public NetworkObject childPrefab;
        public ushort spawnCount = 1;
        public ESpawnForwardMode spawnForwardMode;

        public ProjectileContext Context { get; protected set; }

        public void Handle(CollisionMessage message) {
            if (!Context.HasAuthority) return;
            var rotation = spawnForwardMode switch {
                ESpawnForwardMode.CollisionNormal => Quaternion.LookRotation(message.normal, Vector3.up),
                _ => transform.rotation,
            };
            for (int i = 0; i < spawnCount; i++) {
                FireChild(message.position, rotation);
            }
        }

        public void FireChild(in Vector3 position, in Quaternion rotation) {
            var nob = Context.SpawnManager.InstantiateAndSpawn(childPrefab, position: position, rotation: rotation);
            nob.GetComponent<ProjectileContext>().ownerReference.Value = Context.ownerReference.Value;
        }

        protected void Awake() {
            Context = GetComponentInParent<ProjectileContext>();
        }

        public enum ESpawnForwardMode {
            ParentForward,
            CollisionNormal
        }
    }
}