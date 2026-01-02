using JoG.States;

namespace JoG.StateMachines {

    public interface IStateMachine {
        State CurrentState { get; }

        void TransitionTo(State state);
    }
}