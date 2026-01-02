using EditorAttributes;
using GuestUnion.Extensions;
using GuestUnion.ObjectPool.Generic;
using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace JoG.Projectiles {

    public class ProjectileExplosion : NetworkBehaviour {
        [Required] public ProjectileDamageData damageData;
        [Min(0)] public float damageCoefficient = 1.0f;
        [Min(0)] public float force;
        [Min(0)] public float explosionRadius = 1.0f;
        public LayerMask collisionLayer;
        [Inject] internal IDamageService _damageService;
        [field: SerializeField] public UnityEvent OnDetonated { get; private set; } = new();

        public void Detonate() {
            var position = transform.position;
            var buffer = ArrayPool<Collider>.Shared.Rent(64);
            var count = Physics.OverlapSphereNonAlloc(position, explosionRadius, buffer, collisionLayer);
            if (count > 0) {
                var damageMessage = new DamageMessage {
                    attacker = damageData.Attacker,
                    flags = damageData.Flags | DamageFlags.Physical | DamageFlags.Fire,
                };
                using (DictionaryPool<ulong, ExplosionHit>.Rent(out var idToHit)) {
                    foreach (var collider in buffer.AsSpan(0, count)) {
                        if (!collider.TryGetComponent<IDamageable>(out var damageable)) {
                            continue;
                        }
                        var hitPoint = collider.ClosestPoint(position);
                        var sqrDistance = hitPoint.SqrDistanceTo(position);
                        if (idToHit.TryGetValue(damageable.Id, out var existing) && sqrDistance >= existing.sqrDistance) {
                            continue;
                        }
                        idToHit[damageable.Id] = new ExplosionHit {
                            hitPoint = hitPoint,
                            sqrDistance = sqrDistance,
                            damageable = damageable
                        };
                    }
                    var baseDamage = damageData.Damage * damageCoefficient;
                    foreach (var hit in idToHit.Values) {
                        var distance = Mathf.Sqrt(hit.sqrDistance);
                        var coefficient = 1f - (distance / explosionRadius);
                        damageMessage.value = (int)(coefficient * baseDamage);
                        damageMessage.position = hit.hitPoint;
                        damageMessage.impulse = (hit.hitPoint - position).ChangeMagnitude(force * coefficient);
                        _damageService.DealDamage(hit.damageable, damageMessage);
                    }
                }
            }
            ArrayPool<Collider>.Shared.Return(buffer);
            OnDetonated.Invoke();
        }

        protected void Reset() {
            collisionLayer = LayerMask.GetMask("Default", "CharacterPart", "Dynamic");
        }

        protected void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}