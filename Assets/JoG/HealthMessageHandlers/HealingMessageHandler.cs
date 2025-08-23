using UnityEngine;

namespace JoG.HealthMessageHandlers {

    public abstract class HealingMessageHandler : MonoBehaviour {

        public abstract void Handle(in HealingMessage message);
    }
}