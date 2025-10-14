using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Projectiles {

    public class ProjectileLifetime : NetworkBehaviour, INetworkUpdateSystem {
        public float lifetime = 10f;
        public float remainingLifetime;

        public float Lifetime { get => lifetime; set => lifetime = value; }
        public float RemainingLifetime { get => remainingLifetime; set => remainingLifetime = value; }

        public override void OnNetworkSpawn() {
            remainingLifetime = lifetime;
            this.RegisterNetworkUpdate();
        }

        void INetworkUpdateSystem.NetworkUpdate(NetworkUpdateStage updateStage) {
            if (remainingLifetime > 0) {
                remainingLifetime -= Time.deltaTime;
            }
            if (HasAuthority && IsSpawned && remainingLifetime <= 0) {
                NetworkObject.Despawn();
            }
        }

        public override void OnNetworkDespawn() {
            this.UnregisterNetworkUpdate();
        }
    }
}