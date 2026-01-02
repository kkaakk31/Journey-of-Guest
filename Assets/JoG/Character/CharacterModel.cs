using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;

namespace JoG.Character {

    [Serializable]
    public class CharacterModel : MonoBehaviour {
        [Inject] internal IEnumerable<IVisibleChangedHandler> visibleChangedHandlers;
        private Transform _transform;
        private Vector3 _nameplateOffset;
        private Vector3 _topOffset;
        private Vector3 _centerOffset;
        private Vector3 _bottomOffset;
        [field: SerializeField] public Renderer MainRenderer { get; private set; }
        [field: SerializeField] public Collider MainCollider { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        public Vector3 NameplatePosition => _transform.position + _nameplateOffset;
        public Vector3 Top => _transform.position + _topOffset;
        public Vector3 Center => _transform.position + _centerOffset;
        public Vector3 Bottom => _transform.position + _bottomOffset;
        public float Radius { get; private set; }
        public float Height { get; private set; }

        private void Awake() {
            _transform = transform;
            Assert.AreEqual(_transform, MainCollider.transform);
            var bounds = MainCollider.bounds;
            Height = bounds.size.y;
            Radius = Height * 0.5f;
            if (MainCollider is CapsuleCollider capsuleCollider) {
                _centerOffset = capsuleCollider.center;
            } else if (MainCollider is BoxCollider boxCollider) {
                _centerOffset = boxCollider.center;
            } else {
                Assert.IsNull(MainCollider);
            }
            _nameplateOffset = _centerOffset + new Vector3(0, 1.2f * Radius, 0);
            _topOffset = _centerOffset + new Vector3(0, Radius, 0);
            _bottomOffset = _centerOffset - new Vector3(0, Radius, 0);
        }

        private void OnBecameVisible() {
            foreach (var handler in visibleChangedHandlers) {
                handler.OnBecameVisible();
            }
        }

        private void OnBecameInvisible() {
            foreach (var handler in visibleChangedHandlers) {
                handler.OnBecameInvisible();
            }
        }
    }
}