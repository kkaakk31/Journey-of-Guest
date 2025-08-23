using EditorAttributes;
using GuestUnion.ObjectPool.Generic;
using JoG.HealthMessageHandlers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace JoG.HitColliders {

    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class CharacterCollider : MonoBehaviour, IDamageable, IHealable {
        [Clamp(0.001f, 1000)] public float damageCofficient = 1f;
        [Clamp(0.001f, 1000)] public float healingCofficient = 1f;
        [field: SerializeField, Required] public HealthMessageProcessor Processor { get; private set; }
        [field: SerializeField] public List<DamageMessageHandler> Handlers { get; private set; }

        public void AddHealing(HealingMessage message) {
            message.value = (uint)(message.value * damageCofficient);
            Processor.AddHealing(message);
        }

        public void AddDamage(DamageMessage message) {
            message.value = (uint)(message.value * healingCofficient);
            foreach (var handler in Handlers.AsSpan()) {
                handler.Handle(message);
            }
            Processor.AddDamage(message);
        }

        public void SubmitDamage() {
            Processor.SubmitDamage();
        }

        public void SubmitHealing() {
            Processor.SubmitDamage();
        }

        protected void Awake() {
            Handlers ??= new();
        }

        protected void Reset() {
            Processor = GetComponentInParent<HealthMessageProcessor>();
        }
    }
}