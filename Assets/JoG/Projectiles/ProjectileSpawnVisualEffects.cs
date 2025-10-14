using EditorAttributes;
using JoG.ObjectPool;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Projectiles {

    public class ProjectileSpawnVisualEffects : NetworkBehaviour {
        [Required] public PooledObject pool;

        protected override void OnNetworkPreSpawn(ref NetworkManager networkManager) {
            transform.GetPositionAndRotation(out var position, out var rotation);
            pool.Rent(position, rotation);
        }
    }
}