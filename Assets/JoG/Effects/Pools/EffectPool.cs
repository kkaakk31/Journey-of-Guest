using JoG.ObjectPool;
using UnityEngine;

namespace JoG.VisualEffects.Pools {

    [CreateAssetMenu(fileName = nameof(EffectPool), menuName = nameof(ObjectPool) + "/" + nameof(EffectPool))]
    public class EffectPool : ComponentPool<DespawnOnParticleStopped> {
    }
}