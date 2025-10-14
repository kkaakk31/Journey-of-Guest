using System;
using UnityEngine;

namespace JoG.Projectiles {

    [Serializable]
    public struct CollisionMessage {
        public Collider collider;

        public Rigidbody rigidbody;

        public Vector3 position;

        public Vector3 normal;

        public CollisionMessage(in RaycastHit hit) {
            collider = hit.collider;
            rigidbody = collider.attachedRigidbody;
            position = hit.point;
            normal = hit.normal;
        }

        public CollisionMessage(in ContactPoint contactPoint) {
            collider = contactPoint.otherCollider;
            rigidbody = collider.attachedRigidbody;
            position = contactPoint.point;
            normal = contactPoint.normal;
        }

        public static implicit operator CollisionMessage(in RaycastHit hit) => new(hit);

        public static implicit operator CollisionMessage(in ContactPoint contactPoint) => new(contactPoint);
    }
}