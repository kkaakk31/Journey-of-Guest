using GuestUnion.Extensions.Unity;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace JoG.Character {

    [DisallowMultipleComponent]
    public class CharacterMaster : NetworkBehaviour {
        [field: SerializeField] public UnityEvent<CharacterBody> OnBodyAttached { get; private set; }
        [field: SerializeField] public UnityEvent<CharacterBody> OnBodyDetached { get; private set; }

        public CharacterBody AttachedBody { get; private set; }

        public void AttachBody(CharacterBody body) {
            if (body == null) {
                this.LogError("body to attach is null");
                return;
            }
            if (AttachedBody != null) {
                this.LogError("this master has already attached to a body");
                return;
            }
            AttachedBody = body;
            OnBodyAttach(body);
            OnBodyAttached.Invoke(body);
        }

        public void DetachBody(CharacterBody body) {
            if (body == null) {
                this.LogError("body to detach is null");
                return;
            }
            if (AttachedBody != body) {
                this.LogError("body to detach has not attached to this master");
                return;
            }
            AttachedBody = null;
            OnBodyDetach(body);
            OnBodyDetached.Invoke(body);
        }

        protected virtual void OnBodyAttach(CharacterBody body) {
        }

        protected virtual void OnBodyDetach(CharacterBody body) {
        }

        protected virtual void Awake() {
            OnBodyAttached ??= new();
            OnBodyDetached ??= new();
        }
    }
}