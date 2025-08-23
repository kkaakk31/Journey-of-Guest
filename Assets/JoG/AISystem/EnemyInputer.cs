using EditorAttributes;
using JoG.Character;
using JoG.Character.InputBanks;
using JoG.Messages;
using MessagePipe;
using System;
using System.Buffers;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace JoG.AISystem {

    public class EnemyInputer : MonoBehaviour, IMessageHandler<CharacterBodyChangedMessage> {
        public Transform target;
        public LayerMask targetMask;
        public NavMeshAgent agent;
        private CharacterBody _body;
        private Vector3InputBank _moveInputBank;
        private Vector3InputBank _aimInputBank;
        private IDisposable _disposed;

        [Inject]
        public void Construct(ISubscriber<CharacterBodyChangedMessage> subscriber) {
            _disposed = subscriber.Subscribe(this);
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        void IMessageHandler<CharacterBodyChangedMessage>.Handle(CharacterBodyChangedMessage message) {
            if (message.changeType == CharacterBodyChangeType.Get) {
                _body = message.body;
                _moveInputBank = _body.GetInputBank<Vector3InputBank>("Move");
                _aimInputBank = _body.GetInputBank<Vector3InputBank>("Aim");
            } else {
                _body = null;
                _moveInputBank = null;
                _aimInputBank = null;
            }
        }

        private void Update() {
            if (_body == null) {
                return;
            }
            if (target == null) {
                var colliders = ArrayPool<Collider>.Shared.Rent(10);
                var count = Physics.OverlapSphereNonAlloc(_body.Center, 50f, colliders, targetMask);
                for (var i = 0; i < count; i++) {
                    var body = colliders[i].GetComponentInParent<CharacterBody>();
                    if (body != null && !body.CompareTag(_body.tag)) {
                        target = body.AimOriginTransform;
                        break;
                    }
                }
                ArrayPool<Collider>.Shared.Return(colliders);
            }
            if (target != null) {
                agent.nextPosition = _body.Center;
                _aimInputBank.vector3 =
                    agent.destination = target.position;
            }
            var desiredVelocity = _body.Model.Center.InverseTransformDirection(agent.desiredVelocity);
            _moveInputBank.vector3 = desiredVelocity.sqrMagnitude > 1f
                ? desiredVelocity.normalized
                : desiredVelocity;
        }

        private void OnDestroy() {
            _disposed.Dispose();
        }
    }
}