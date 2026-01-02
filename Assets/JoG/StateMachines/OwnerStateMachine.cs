using JoG.States;
using Unity.Netcode;
using UnityEngine;

namespace JoG.StateMachines {

    [DisallowMultipleComponent]
    public class OwnerStateMachine : NetworkBehaviour, IStateMachine {
        public State entryState;
        private State _currentState;

        public State CurrentState => _currentState;

        public void TransitionTo(State state) {
            _currentState?.Exit();
            state?.Enter();
            _currentState = state;
        }

        public override void OnNetworkSpawn() {
            if (IsOwner) {
                TransitionTo(entryState);
            }
        }

        public override void OnNetworkDespawn() {
            if (IsOwner) {
                TransitionTo(null);
            }
        }

        protected override void OnOwnershipChanged(ulong previous, ulong current) {
            if (IsOwner) {
                TransitionTo(entryState);
            } else {
                TransitionTo(null);
            }
        }
    }
}