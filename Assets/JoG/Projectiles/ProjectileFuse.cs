using UnityEngine;
using UnityEngine.Events;

namespace JoG.Projectiles {

    public class ProjectileFuse : MonoBehaviour {
        public float fuseTime = 3f;
        private float _fuseEndTime;
        private bool _fuseEnded;
        [field: SerializeField] public UnityEvent OnFuseEnd { get; private set; } = new();

        protected void OnEnable() {
            _fuseEndTime = Time.time + fuseTime;
            _fuseEnded = false;
        }

        protected void Update() {
            if (_fuseEnded || Time.time < _fuseEndTime) return;
            OnFuseEnd.Invoke();
            _fuseEnded = true;
        }
    }
}