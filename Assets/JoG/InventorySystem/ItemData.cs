using JoG.Localization;
using UnityEngine;

namespace JoG.InventorySystem {

    [CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory/ItemData")]
    public class ItemData : ScriptableObject {
        public string nameToken;
        public string descriptionToken;
        public Sprite iconSprite;
        public GameObject prefab;
        public string Name => Localizer.GetString(nameToken);
        public string Description => Localizer.GetString(descriptionToken);
    }
}