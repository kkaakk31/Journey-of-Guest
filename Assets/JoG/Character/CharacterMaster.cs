using EditorAttributes;
using JoG.Messages;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace JoG.Character {

    [DisallowMultipleComponent]
    public class CharacterMaster : NetworkBehaviour {
        [Inject] private IBufferedPublisher<CharacterBodyChangedMessage> _publisher;

        public CharacterBody Body { get; private set; }

        public virtual void OnBodyChanged(in CharacterBodyChangedMessage message) {
            Body = message.body;
            _publisher.Publish(message);
        }

        //protected virtual void OnTransformChildrenChanged() {
        //    var body = GetComponentInChildren<CharacterBody>();
        //    if (Body == body) return;
        //    if (Body != null) {
        //        var message = new CharacterBodyChangedMessage {
        //            changeType = CharacterBodyChangeType.Lose,
        //            body = Body,
        //        };
        //        Body = null;
        //        _publisher.Publish(message);
        //        OnBodyChanged(message);
        //    }
        //    Body = body;
        //    if (body != null) {
        //        var message = new CharacterBodyChangedMessage {
        //            changeType = CharacterBodyChangeType.Get,
        //            body = body,
        //        };
        //        _publisher.Publish(message);
        //        OnBodyChanged(message);
        //    }
        //}
    }
}