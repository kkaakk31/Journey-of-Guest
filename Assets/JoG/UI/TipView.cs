using EditorAttributes;
using TMPro;
using UnityEngine;

namespace JoG.UI {

    public class TipView : MonoBehaviour {
        [field: SerializeField, Required] public TMP_Text NameText { get; private set; }
        [field: SerializeField, Required] public TMP_Text DescriptionText { get; private set; }

        public string TipName {
            get => NameText.text;
            set => NameText.text = value;
        }

        public string TipDescription {
            get => DescriptionText.text;
            set => DescriptionText.text = value;
        }

        public void SetTip(string name, string description) {
            NameText.text = name;
            DescriptionText.text = description;
        }

        public void UpdateView(GameObject gameObject) {
            var tipName = string.Empty;
            var tipDescription = string.Empty;
            if (gameObject != null) {
                if (gameObject.TryGetComponent<OutlineController>(out var outlineController)) {
                    outlineController.ShowOutline(0.1f);
                }
                if (gameObject.TryGetComponent<IInformationProvider>(out var provider)) {
                    tipName = provider.Name;
                    tipDescription = provider.Description;
                }
            }
            SetTip(tipName, tipDescription);
        }
    }
}