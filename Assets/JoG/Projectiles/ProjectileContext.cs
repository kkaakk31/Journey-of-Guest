using Unity.Netcode;
using UnityEngine;

namespace JoG.Projectiles {

    [DisallowMultipleComponent]
    public class ProjectileContext : NetworkBehaviour {
        public readonly NetworkVariable<NetworkObjectReference> ownerReference = new(writePerm: NetworkVariableWritePermission.Owner);
        public NetworkSpawnManager SpawnManager { get; private set; }

        public new NetworkManager NetworkManager { get; private set; }

        public NetworkObject OwnerObject {
            get => ownerReference.Value;
            set => ownerReference.Value = new(value);
        }

        protected void Awake() {
            NetworkManager = base.NetworkManager;
            SpawnManager = NetworkManager.SpawnManager;
        }
    }
}