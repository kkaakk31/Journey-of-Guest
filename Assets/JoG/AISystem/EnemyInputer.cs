using JoG.Character;
using JoG.Character.InputBanks;
using JoG.Messages;
using MessagePipe;
using System;
using UnityEngine;
using VContainer;

namespace JoG.AISystem {

    public class EnemyInputer : MonoBehaviour, IMessageHandler<CharacterBodyChangedMessage> {
        public Transform target;
        private CharacterBody _body;
        private Vector3InputBank _moveInputBank;
        private IDisposable _disposed;

        [Inject]
        public void Construct(ISubscriber<CharacterBodyChangedMessage> subscriber) {
            _disposed = subscriber.Subscribe(this);
        }

        void IMessageHandler<CharacterBodyChangedMessage>.Handle(CharacterBodyChangedMessage message) {
            if (message.changeType == CharacterBodyChangeType.Get) {
                _body = message.body;
                _moveInputBank = _body.GetInputBank<Vector3InputBank>("Move");
            } else {
                _body = null;
                _moveInputBank = null;
            }
            target = null;
        }

        private void OnDestroy() {
            _disposed.Dispose();
        }
    }
}