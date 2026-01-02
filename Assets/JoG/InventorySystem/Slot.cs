using EditorAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoG.InventorySystem {

    public partial class Slot : MonoBehaviour {
        [Required] public Image iconImage;
        [Required] public TextMeshProUGUI countText;
    }
}