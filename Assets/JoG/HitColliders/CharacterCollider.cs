using UnityEngine;

namespace JoG.HitColliders {

    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class CharacterCollider : MonoBehaviour, IDamageable, IHealable {
        public float damageCofficient = 1f;
        public float healingCofficient = 1f;
        public IHealth Health { get; private set; }
        public DamageMessageHandler DamageMessageHandler { get; private set; }

        public void Handle(HealingMessage message) {
            message.value = (uint)(message.value * damageCofficient);
            Health.Handle(message);
        }

        public void AddDamage(DamageMessage message) {
            message.value = (uint)(message.value * healingCofficient);
            DamageMessageHandler.AddDamage(message);
        }

        public void SubmitDamage() {
            DamageMessageHandler.Submit();
        }

        protected void Awake() {
            Health = GetComponentInParent<IHealth>();
            DamageMessageHandler = GetComponentInParent<DamageMessageHandler>();
        }
    }
}