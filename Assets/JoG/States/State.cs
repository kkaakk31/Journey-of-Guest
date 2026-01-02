using JoG.StateMachines;
using UnityEngine;

namespace JoG.States {

    [DisallowMultipleComponent]
    public class State : MonoBehaviour {
        public IStateMachine StateMachine { get; private set; }

        internal void Enter() {
            gameObject.SetActive(true);
        }

        internal void Exit() {
            gameObject.SetActive(false);
        }

        protected void TransitionTo(State state) => StateMachine.TransitionTo(state);

        protected virtual void Awake() {
            StateMachine = transform.parent.GetComponent<IStateMachine>();
        }
    }
}