using EditorAttributes;
using GuestUnion.YooAsset;
using JoG.Buff.Datas;
using UnityEngine;

namespace JoG.HealthMessageHandlers {

    public class BurningBuffHandler : DamageMessageHandler {
        [SerializeField] private YooAssetReference<DoTData> _dotData;
        public ushort damageCount;

        public override void Handle(in DamageMessage message) {
            if (message.HasFlag(DamageFlags.Fire)) {
                var buff = _dotData.AssetObject.RentDoT();
                buff.Initialize(message.attacker, damageCount, message.value);
            }
        }

        private void Awake() {
            _dotData.LoadAssetSync();
        }

        private void OnDestroy() {
            _dotData.Dispose();
        }
    }
}