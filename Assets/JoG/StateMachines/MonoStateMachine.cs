using JoG.States;
using UnityEngine;

namespace JoG.StateMachines {

    [DisallowMultipleComponent]
    public class MonoStateMachine : MonoBehaviour, IStateMachine {
        public State entryState;
        private State _currentState;

        public State CurrentState => _currentState;

        public void TransitionTo(State state) {
            _currentState?.Exit();
            state?.Enter();
            _currentState = state;
        }

        protected void OnEnable() => TransitionTo(entryState);

        protected void OnDisable() => TransitionTo(null);
    }
}