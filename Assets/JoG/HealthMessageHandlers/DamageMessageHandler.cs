using UnityEngine;

namespace JoG.HealthMessageHandlers {

    public abstract class DamageMessageHandler : MonoBehaviour {

        public abstract void Handle(in DamageMessage message);
    }
}