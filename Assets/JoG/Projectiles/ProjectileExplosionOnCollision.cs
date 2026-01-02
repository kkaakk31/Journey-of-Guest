using GuestUnion.ObjectPool.Unity;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Projectiles {

    public class ProjectileExplosionOnCollision : ProjectileExplosion {
        [field: SerializeField] public PooledObject ExplosionPrefab { get; private set; }

        public void Handle(CollisionMessage message) {
            if (!HasAuthority) return;
            SpawnFXRpc(message.position, Quaternion.LookRotation(message.normal, Vector3.up));
            Detonate();
        }

        [Rpc(SendTo.Everyone)]
        protected void SpawnFXRpc(Vector3 position, Quaternion rotation) {
            ExplosionPrefab.Rent(position, rotation);
        }
    }
}