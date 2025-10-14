using System.Collections;
using UnityEngine;

namespace JoG.Projectiles {

    public class RigidbodyProjectileMover : MonoBehaviour, IProjectileMover {
        [SerializeField] protected Rigidbody _rigidbody;

    }
}