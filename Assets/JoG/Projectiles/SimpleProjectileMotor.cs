using Unity.Netcode;
using UnityEngine;

namespace JoG.Projectiles {

    [RequireComponent(typeof(ProjectileData))]
    public class SimpleProjectileMotor : NetworkBehaviour, IProjectileMotor, INetworkUpdateSystem {
        public const float MAX_PASSED_TIME = 0.3f;

        [Range(0, 1000f)]
        public float lifetime = 10f;

        [Range(0, 3e+8f)]
        public float speed = 100f;

        [Range(float.Epsilon, 1000f)]
        public float radius = 0.1f;

        [Tooltip("network delay")]
        protected float passedTime;

        private float _deadtime;
        private IProjectileHitMessageHandler _messageHandler;
        public ProjectileData Data { get; private set; }

        public override void OnNetworkSpawn() {
            this.RegisterNetworkUpdate();
        }

        public override void OnNetworkDespawn() {
            this.UnregisterNetworkUpdate();
        }

        void INetworkUpdateSystem.NetworkUpdate(NetworkUpdateStage updateStage) {
            var delta = Time.deltaTime;
            if (passedTime > 0) {
                delta += passedTime;
                passedTime = 0;
                GetComponentInChildren<ParticleSystem>().time += passedTime;
            }
            transform.GetPositionAndRotation(out var position, out var rotation);
            var distance = speed * delta;
            var forward = rotation * Vector3.forward;
            if (HasAuthority) {
                if (Time.time >= _deadtime) {
                    NetworkObject.DeferDespawn(4);
                    return;
                }
                if (Physics.SphereCast(position, radius, forward, out var hitInfo, distance, Data.collisionLayer, QueryTriggerInteraction.Collide)) {
                    _messageHandler.Handle(hitInfo);
                    NetworkObject.DeferDespawn(4);
                    return;
                }
            }
            transform.position = position + (distance * forward);
        }

        protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer) {
            if (serializer.IsWriter) {
                passedTime = 0;
                _deadtime = Time.time + lifetime;
                var startTick = NetworkManager.LocalTime.Tick;
                serializer.SerializeValue(ref startTick);
            } else {
                var startTick = 0;
                serializer.SerializeValue(ref startTick);
                passedTime = Mathf.Min(NetworkManager.LocalTime.TimeTicksAgo(startTick).TimeAsFloat, MAX_PASSED_TIME);
            }
        }

        protected void Awake() {
            _messageHandler = GetComponent<IProjectileHitMessageHandler>();
            Data = GetComponent<ProjectileData>();
        }
    }
}