using JoG.Character;
using System.Collections.Generic;
using Unity.Netcode;

namespace JoG.AISystem {

    public class EnemySpawner : NetworkBehaviour {
        public List<EnemyMaster> masters = new();

        protected override void OnNetworkSessionSynchronized() {
            if (!HasAuthority) {
                return;
            }
            foreach (var master in masters) {
                if (master == null) {
                    continue;
                }
                if (master.Body != null && master.Body.IsSpawned) {
                    continue;
                }
                master.SpawnBody();
            }
        }
    }
}