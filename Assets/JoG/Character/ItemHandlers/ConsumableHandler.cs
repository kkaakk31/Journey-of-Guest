using JoG.Character.InputBanks;
using JoG.InventorySystem;
using UnityEngine;

namespace JoG.Character.ItemHandlers {

    public class ConsumableHandler : MonoBehaviour, IItemHandler {
        private CharacterBody _body;
        private TriggerInputBank _useInputBank;
        private ItemUser _controller;

        void IItemHandler.Handle(GameObject item) {
            // 实现消耗品装备/切换逻辑
        }

        private void Awake() {
            _controller = GetComponentInParent<ItemUser>();
        }

        private void Update() {
            if (_useInputBank.Triggered) {
                _controller.ChangeItemCount(1);
            }
        }
    }
}