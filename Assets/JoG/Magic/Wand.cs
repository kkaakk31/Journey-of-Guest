using GuestUnion;
using JoG.Character;
using JoG.Localization;
using System;
using Unity.Netcode;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace JoG.Magic {

    public class Wand : NetworkBehaviour, IInformationProvider {
        public Spell spell;
        public uint manaCapacity = 100;
        public LocalizableString localizableName;
        public LocalizableString localizableDescription;
        [NonSerialized] public float value;
        [Range(1, float.MaxValue)] public float chargingSpeed;
        [Range(0.02f, 50f)] public float cooldown = 1f;
        public Vector2 maxSpread;
        private float _lastCastTime;
        private NetworkVariable<NetworkObjectReference> _ownerReference = new();

        public NetworkObject Owner {
            set => _ownerReference.Value = value;
        }

        [field: SerializeField] public Transform SpawnPoint { get; private set; }
        [field: SerializeField] public AudioSource AudioSource { get; private set; }
        public Vector2 CurrentRandomSpread => URandom.insideUnitCircle * maxSpread;

        public string Name => localizableName.Value;

        public string Description => localizableDescription.Value;

        public string GetProperty(string token) => string.Empty;

        public virtual void Cast(CharacterBody caster) {
            if (spell.manaCost > value || (Time.time - _lastCastTime) < cooldown) return;
            value -= spell.manaCost;
            SpawnPoint.GetPositionAndRotation(out var position, out var rotation);
            var spread = CurrentRandomSpread;
            spell.Cast(caster, position, rotation.ApplySpread(spread.y, spread.x));
            if (spell.spellClip != null) {
                AudioSource.PlayOneShot(spell.spellClip);
            }
            _lastCastTime = Time.time;
        }

        private void Update() {
            if (value < manaCapacity) {
                value += chargingSpeed * Time.deltaTime;
                if (value > manaCapacity) {
                    value = manaCapacity;
                }
            }
        }
    }
}