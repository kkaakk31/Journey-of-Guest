using System;
using UnityEngine.Events;

namespace JoG.Projectiles {

    [Serializable]
    public class CollisionEvent : UnityEvent<CollisionMessage> {
    }
}