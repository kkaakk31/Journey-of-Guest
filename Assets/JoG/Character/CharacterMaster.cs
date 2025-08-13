using JoG.Messages;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace JoG.Character {

    [DisallowMultipleComponent]
    public class CharacterMaster : NetworkBehaviour {
        [Inject] private IBufferedPublisher<CharacterBodyChangedMessage> _bodyChangedMessagePublisher;

        public CharacterBody Body { get; private set; }

        protected virtual void OnBodyChanged(in CharacterBodyChangedMessage message) {
            //if (message.changeType is CharacterBodyChangeType.Lose && NetworkObject.IsSpawned) {
            //    NetworkObject.Despawn();
            //}
        }

        protected virtual void OnTransformChildrenChanged() {
            var body = GetComponentInChildren<CharacterBody>();
            if (Body == body) return;
            if (Body != null) {
                var message = new CharacterBodyChangedMessage {
                    changeType = CharacterBodyChangeType.Lose,
                    body = Body,
                };
                _bodyChangedMessagePublisher.Publish(message);
                OnBodyChanged(message);
            }
            if (body != null) {
                var message = new CharacterBodyChangedMessage {
                    changeType = CharacterBodyChangeType.Get,
                    body = body,
                };
                _bodyChangedMessagePublisher.Publish(message);
                OnBodyChanged(message);
            }
            Body = body;
        }
    }
}