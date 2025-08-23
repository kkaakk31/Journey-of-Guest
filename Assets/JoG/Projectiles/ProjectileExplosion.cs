using GuestUnion;
using GuestUnion.ObjectPool.Generic;
using JoG.DebugExtensions;
using System.Buffers;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Projectiles {

    [RequireComponent(typeof(ProjectileData))]
    public class ProjectileExplosion : MonoBehaviour, IProjectileHitMessageHandler {
        public uint damageValue;
        public float force;
        public float explosionRadius;
        public LayerMask damageLayer;
        public NetworkObject explosionEffectPrefab;
        private ProjectileData _data;
        private DamageMessage damageMessage;

        public void Handle(in ProjectileHitMessage message) {
            if (_data.HasAuthority) {
                var buffer = ArrayPool<Collider>.Shared.Rent(30);
                var count = Physics.OverlapSphereNonAlloc(
                    message.position,
                    explosionRadius,
                    buffer,
                    damageLayer);
                if (count is not 0) {
                    damageMessage.position = message.position;
                    using (ListPool<IDamageable>.Rent(out var damageables)) {
                        for (var i = 0; i < count; ++i) {
                            var collider = buffer[i];
                            var direction = collider.transform.position - message.position;
                            if (Physics.Raycast(message.position, direction, out var hit, explosionRadius, _data.collisionLayer)) {
                                if (hit.collider == collider && collider.TryGetComponent<IDamageable>(out var damageable)) {
                                    var distance = Vector3.Distance(message.position, hit.point);
                                    var confficient = 1 - (distance / explosionRadius).Clamp01();
                                    damageMessage.value = (uint)(damageValue * confficient);
                                    damageMessage.impulse = direction.normalized * (force * confficient);
                                    damageable.AddDamage(damageMessage);
                                    damageables.Add(damageable);
                                }
                            }
                        }
                        foreach (var damageable in damageables.AsSpan()) {
                            damageable.SubmitDamage();
                        }
                    }
                }
                ArrayPool<Collider>.Shared.Return(buffer);
            }
            explosionEffectPrefab.InstantiateAndSpawn(
                _data.NetworkManager,
                position: message.position,
                rotation: Quaternion.LookRotation(message.normal));
        }

        protected void Awake() {
            _data = GetComponent<ProjectileData>();
            damageMessage = new DamageMessage() {
                value = damageValue,
                cofficient = 1,
                flags = DamgeFlag.magic | DamgeFlag.fire,
                attacker = _data.ownerReference,
            };
        }
    }
}