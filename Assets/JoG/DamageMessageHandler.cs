using UnityEngine;

namespace JoG {

    public class DamageMessageHandler : MonoBehaviour {
        public bool hasValue;
        public DamageMessage damageMessage;
        private IHealth _health;

        public void AddDamage(in DamageMessage message) {
            if (hasValue) {
                damageMessage.value += message.value;
                damageMessage.impulse += message.impulse;
                damageMessage.flags |= message.flags;
                damageMessage.attacker = message.attacker;
            } else {
                damageMessage = message;
                hasValue = true;
            }
        }

        public void Submit() {
            if (hasValue) {
                damageMessage.cofficient = 1;
                _health.Handle(damageMessage);
                hasValue = false;
            }
        }

        private void Awake() {
            _health = GetComponentInParent<IHealth>();
        }
    }
}