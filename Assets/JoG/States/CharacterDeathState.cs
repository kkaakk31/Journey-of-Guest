using JoG.States;
using UnityEngine;

namespace JoG.Character {

    public class CharacterDeathState : State {
        public float despawnDelay = 5;
        private float _despawnTime;
        private CharacterBody _body;

        protected override bool CheckTransitionIn() => !_body.IsAlive;

        protected override bool CheckTransitionOut() => _body.IsAlive;

        protected override void Awake() {
            base.Awake();
            _body = GetComponentInParent<CharacterBody>();
        }

        protected void OnEnable() {
            _despawnTime = Time.time + despawnDelay;
            _body.Animator.SetBool(AnimationParameters.isDead, true);
        }

        protected override void Update() {
            base.Update();
            if (Time.time > _despawnTime) {
                _body.NetworkObject.Despawn();
            }
        }

        protected void OnDisable() {
            _body.Animator.SetBool(AnimationParameters.isDead, false);
        }
    }
}