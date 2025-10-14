using EditorAttributes;
using JoG.Buffs;
using JoG.BuffSystem;
using JoG.Character;
using UnityEngine;

namespace JoG.HealthMessageHandlers {

    public class BurningBuffHandler : DamageMessageHandler {
        [Clamp(0.001f, 1000)] public float damageCofficient = 0.5f;
        public ushort damageCount = 5;
        [SerializeField, Required] private HealthMessageProcessor _processor;

        public override void Handle(in DamageMessage message) {
            if (DamgeFlag.HasFlag(message, DamgeFlag.fire)) {
                var buff = BuffPool.Rent<BurningBuff>();
                buff.attacker = message.attacker;
                buff.damageValuePerTick = (uint)(message.value * damageCofficient);
                if (buff.damageValuePerTick < 1) {
                    buff.damageValuePerTick = 1;
                }
                buff.damageCount = damageCount;
                _processor.AddBuff(buff);
            }
        }
    }
}