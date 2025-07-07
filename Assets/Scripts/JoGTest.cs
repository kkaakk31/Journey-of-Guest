using JoG.DebugExtensions;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace JoG {

    public class JoGTest : NetworkBehaviour {
        private void Awake() {
            this.Log(transform.position);
        }

        private void Start() {
            this.Log(transform.position);
        }

        protected override void OnNetworkPostSpawn() {
            base.OnNetworkPostSpawn();
            this.Log(transform.position);
        }
         
    }
}