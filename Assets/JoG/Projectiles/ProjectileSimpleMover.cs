using EditorAttributes;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Projectiles {

    public class ProjectileSimpleMover : NetworkBehaviour, IProjectileMover, INetworkUpdateSystem {

        [Clamp(0, 3e+8f)]
        public float speed = 100f;

        [Clamp(0.001f, 1000f)]
        public float detectionRadius = 0.01f;

        public LayerMask collisionLayer;
        protected bool _collided = false;

        [field: SerializeField] public CollisionEvent OnCollision { get; private set; } = new();

        public override void OnNetworkSpawn() {
            _collided = false;
            this.RegisterNetworkUpdate();
        }

        void INetworkUpdateSystem.NetworkUpdate(NetworkUpdateStage updateStage) {
            if (!HasAuthority || _collided) return;
            transform.GetPositionAndRotation(out var position, out var rotation);
            var distance = speed * Time.deltaTime;
            var forward = rotation * Vector3.forward;
            if (Physics.SphereCast(position, detectionRadius, forward, out var hitInfo, distance, collisionLayer, QueryTriggerInteraction.Collide)) {
                OnCollision.Invoke(hitInfo);
                transform.position = hitInfo.point;
                _collided = true;
                return;
            }
            transform.position = position + (distance * forward);
        }

        public override void OnNetworkDespawn() {
            this.UnregisterNetworkUpdate();
        }

        protected void OnEnable() {
            transform.localPosition = Vector3.zero;
        }

        protected void OnDisable() {
            transform.localPosition = Vector3.zero;
        }

        protected void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}