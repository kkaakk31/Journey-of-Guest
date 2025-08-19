using EditorAttributes;
using TMPro;
using UnityEngine;

namespace JoG.UI {

    public class CharacterNameplate : MonoBehaviour {
        [SerializeField, Required] private TMP_Text _label;
        [SerializeField, Required] private GameObject _nameplate;

        public string NameplateText {
            get => _label.text;
            set => _label.text = value;
        }
    }
}