using GuestUnion.Extensions;
using GuestUnion.Extensions.Unity;
using GuestUnion.ObjectPool.Generic;
using GuestUnion.YooAsset;
using Unity.Netcode;
using UnityEngine;

namespace JoG.Character {

    public class EnemyMaster : CharacterMaster {
        public ushort lifeCount = 1;
        public YooAssetReference<GameObject> characterHUDPrefab;
        private GameObject _characterHUD;

        public void SpawnBody(NetworkObject bodyPrefab, in Vector3 position, in Quaternion rotation) {
            if (!HasAuthority) {
                this.LogError("No authority to spawn body.");
                return;
            }
            if (AttachedBody != null && AttachedBody.IsSpawned) {
                this.LogError("Body is already spawned.");
                return;
            }
            if (bodyPrefab == null) {
                this.LogError("Body prefab is null.");
            }
            var nob = NetworkManager.SpawnManager.InstantiateAndSpawn(
                bodyPrefab,
                destroyWithScene: true,
                isPlayerObject: false,
                position: position,
                rotation: rotation
            );
            if (!nob.TrySetParent(NetworkObject, true)) {
                nob.Despawn();
            }
        }

        protected override void OnBodyAttach(CharacterBody body) {
            body.tag = tag;
            body.OnHandleDamageReport += OnHandleDamageReport;
            if (_characterHUD == null) {
                _characterHUD = Instantiate(characterHUDPrefab.AssetObject, transform);
                using (ListPool<IBodyAttachHandler>.Rent(out var list)) {
                    _characterHUD.GetComponentsInChildren(true, list);
                    foreach (var handler in list) {
                        OnBodyAttached.AddListener(handler.OnBodyAttached);
                        OnBodyDetached.AddListener(handler.OnBodyDetached);
                    }
                }
                using (ListPool<Canvas>.Rent(out var list)) {
                    var uiCamera = Camera.allCameras.Find(c => c.CompareTag("UICamera"));
                    _characterHUD.GetComponentsInChildren(true, list);
                    foreach (var canvas in list) {
                        canvas.worldCamera = uiCamera;
                    }
                }
            }
            _characterHUD.SetActive(true);
        }

        protected override void OnBodyDetach(CharacterBody body) {
            body.OnHandleDamageReport -= OnHandleDamageReport;
            _characterHUD.SetActive(false);
        }

        protected override void Awake() {
            base.Awake();
            characterHUDPrefab.LoadAssetSync();
        }

        private void OnHandleDamageReport(in DamageReport report) {
            if (HasAuthority && report.killed && lifeCount > 0) {
                lifeCount--;
                if (lifeCount == 0) {
                    this.Log("Enemy has no more lives.");
                } else {
                    this.Log($"Enemy lost a life. Remaining lives: {lifeCount}");
                    AttachedBody.HP = AttachedBody.MaxHP;
                }
            }
        }
    }
}