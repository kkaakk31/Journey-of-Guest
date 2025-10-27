using EditorAttributes;
using JoG.BuffSystem;
using JoG.Character;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace JoG.HealthMessageHandlers {

    public class HealthMessageProcessor : MonoBehaviour {
        public bool hasDamage;
        public DamageMessage damageMessage;
        public bool hasHealing;
        public HealingMessage healingMessage;
        public List<IBuff> buffCache = new();
        [SerializeField, Required] private CharacterBody _body; 

        public void AddBuff(IBuff buff) {
            foreach (var b in buffCache.AsSpan()) {
                if (b.Index == buff.Index) {
                    b.MergeWith(buff);
                    BuffPool.Return(buff);
                    return;
                }
            }
            buffCache.Add(buff);
        }

        public void AddDamage(in DamageMessage message) {
            if (hasDamage) {
                damageMessage.value += message.value;
                damageMessage.impulse += message.impulse;
                damageMessage.flags |= message.flags;
                damageMessage.attacker = message.attacker;
            } else {
                damageMessage = message;
                hasDamage = true;
            }
        }

        public void AddHealing(in HealingMessage message) {
            if (hasHealing) {
                healingMessage.value += message.value;
                healingMessage.flags |= message.flags;
                healingMessage.healer = message.healer;
            } else {
                healingMessage = message;
                hasHealing = true;
            }
        }

        public void SubmitDamage() {
            if (hasDamage) {
                damageMessage.cofficient = 1;
                _body.Handle(damageMessage);
                hasDamage = false;
            }
            foreach (var buff in buffCache.AsSpan()) {
                _body.AddBuffOnEveryone(buff);
            }
            buffCache.Clear();
        }

        public void SubmitHealing() {
            if (hasHealing) {
                healingMessage.cofficient = 1;
                _body.Handle(healingMessage);
                hasHealing = false;
            }
            foreach (var buff in buffCache.AsSpan()) {
                _body.AddBuffOnEveryone(buff);
            }
            buffCache.Clear();
        }

        protected void Reset() {
            _body = GetComponentInParent<CharacterBody>();
        }
    }
}