using EditorAttributes;
using UnityEngine;

namespace JoG.States {

    public class CharacterSpawningState : State {
        [Required, SerializeField] private Animator _animator;
        [Required, SerializeField] private State _next;

        protected void OnEnable() {
            _animator.SetBool(AnimationParameters.isSpawning, true);
        }

        protected void Update() {
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsTag("SpawnState")) {
                return;
            }
            TransitionTo(_next);
        }

        protected void OnDisable() {
            _animator.SetBool(AnimationParameters.isSpawning, false);
        }
    }
}