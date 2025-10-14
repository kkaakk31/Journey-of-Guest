using EditorAttributes;
using Unity.Netcode;
using UnityEngine;

namespace JoG.VisualEffects {

    public class DespawnOnParticleStopped : MonoBehaviour {
        [field: SerializeField, Required] public NetworkObject NetworkObject { get; private set; }
        [field: SerializeField, Required] public ParticleSystem ParticleSystem { get; private set; }

        protected void OnParticleSystemStopped() {
            if (NetworkObject.HasAuthority) {
                NetworkObject.Despawn();
            }
        }

        protected void Reset() {
            NetworkObject = GetComponentInParent<NetworkObject>();
            ParticleSystem = GetComponent<ParticleSystem>();
        }
    }
}