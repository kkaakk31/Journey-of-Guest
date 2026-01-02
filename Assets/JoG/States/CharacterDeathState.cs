using JoG.States;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace JoG.Character {

    public class CharacterDeathState : State {
        [Inject] internal NetworkObject _networkObject;
        [Inject] internal Animator _body;

        protected void OnEnable() {
            _body.SetBool(AnimationParameters.isDead, true);
        }

        protected void Update() {
            var state = _body.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime > 1f) {
                _networkObject.Despawn();
            }
        }

        protected void OnDisable() {
            _body.SetBool(AnimationParameters.isDead, false);
        }
    }
}