using GuestUnion;
using GuestUnion.ObjectPool.Generic;
using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace JoG.Projectiles {

    public class ProjectileExplosion : NetworkBehaviour {
        public uint damageValue = 16;
        public float force;
        public float explosionRadius = 1;
        public LayerMask damageLayer;
        public LayerMask collisionLayer;
        public ProjectileContext Context { get; private set; }
        [field: SerializeField] public UnityEvent OnDetonated { get; private set; } = new();

        public void Detonate() {
            var position = transform.position;
            var buffer = ArrayPool<Collider>.Shared.Rent(64);
            var count = Physics.OverlapSphereNonAlloc(position, explosionRadius, buffer, damageLayer);
            if (count is not 0) {
                var damageMessage = new DamageMessage {
                    value = damageValue,
                    cofficient = 1,
                    flags = DamgeFlag.magic | DamgeFlag.fire,
                    attacker = Context.ownerReference.Value,
                    position = position
                };
                using (ListPool<IDamageable>.Rent(out var damageables)) {
                    foreach (var collider in buffer.AsSpan(0, count)) {
                        var direction = collider.transform.position - position;
                        if (Physics.Raycast(position, direction, out var hit, explosionRadius, collisionLayer)) {
                            if (hit.collider == collider && collider.TryGetComponent<IDamageable>(out var damageable)) {
                                var distance = Vector3.Distance(position, hit.point);
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
            OnDetonated.Invoke();
        }

        protected void Awake() {
            Context = GetComponentInParent<ProjectileContext>();
        }

        protected void Reset() {
            damageLayer = LayerMask.GetMask("CharacterPart");
            collisionLayer = LayerMask.GetMask("Default", "CharacterPart", "Dynamic");
        }

        protected void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}