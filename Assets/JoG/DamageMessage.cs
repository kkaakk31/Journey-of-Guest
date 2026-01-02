using System;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

namespace JoG {

    [Serializable]
    public struct DamageMessage : INetworkSerializable {
        public const int MaxDamageInt = 10_00000000;
        public const long MaxDamageLong = 10_00000000L;
        public const float MaxDamageFloat = 10_00000000f;

        public static readonly int PreCheckedSize =
            sizeof(ulong) +     // flags
            sizeof(int) +       // value
            sizeof(float) * 3 + // position (x,y,z)
            sizeof(float) * 3;  // impulse (x,y,z)

        public NetworkObjectReference attacker;
        public ulong flags;
        public int value;
        public Vector3 position;
        public Vector3 impulse;

        /// <summary>Scales value by a percentage factor (e.g., 150 = 150% = ×1.5).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ScaleByPercent(short percent) {
            var scaled = (long)value * percent / 100L;
            if (scaled < 0L) value = 0;
            else if (scaled > MaxDamageLong) value = MaxDamageInt;
            else value = (int)scaled;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasFlag(ulong flag) => (flags & flag) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasFlags(ulong flags) => (this.flags & flags) != flags;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeNetworkSerializable(ref attacker);
            if (serializer.PreCheck(PreCheckedSize)) {
                serializer.SerializeValuePreChecked(ref flags);
                serializer.SerializeValuePreChecked(ref value);
                serializer.SerializeValuePreChecked(ref position);
                serializer.SerializeValuePreChecked(ref impulse);
            }
        }
    }
}