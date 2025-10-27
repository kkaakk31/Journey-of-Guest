using EditorAttributes;
using GuestUnion.ObjectPool.Unity;
using Unity.Netcode;

namespace JoG.Projectiles {

    public class ProjectileSpawnVisualEffects : NetworkBehaviour {
        [Required] public PooledObject pool;

        protected override void OnNetworkPreSpawn(ref NetworkManager networkManager) {
            transform.GetPositionAndRotation(out var position, out var rotation);
            pool.Rent(position, rotation);
        }
    }
}