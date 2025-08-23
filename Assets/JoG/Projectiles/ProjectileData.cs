using Unity.Netcode;
using UnityEngine;

namespace JoG.Projectiles {

    public class ProjectileData : NetworkBehaviour {
        public NetworkObjectReference ownerReference;
        public LayerMask collisionLayer;

        public override void OnNetworkDespawn() {
            ownerReference = default;
        }
    }
}