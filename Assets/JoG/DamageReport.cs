using System;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

namespace JoG {

    [Serializable]
    public struct DamageReport : INetworkSerializable {

        public static readonly int PreCheckedSize =
            sizeof(ulong) +     // flags
            sizeof(int) +       // rawDamage
            sizeof(int) +       // deltaDamage
            sizeof(float) * 3 + // position (x,y,z)
            sizeof(float) * 3;  // impulse (x,y,z)

        public NetworkObjectReference attacker;
        public ulong flags;
        public int rawDamage;
        public int deltaDamage;
        public Vector3 impulse;
        public Vector3 position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasFlag(ulong flag) => (flags & flag) != 0;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasFlags(ulong flags) => (this.flags & flags) != flags;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeNetworkSerializable(ref attacker);
            if (serializer.PreCheck(PreCheckedSize)) {
                serializer.SerializeValuePreChecked(ref flags);
                serializer.SerializeValuePreChecked(ref rawDamage);
                serializer.SerializeValuePreChecked(ref deltaDamage);
                serializer.SerializeValuePreChecked(ref position);
                serializer.SerializeValuePreChecked(ref impulse);
            }
        }
    }
}