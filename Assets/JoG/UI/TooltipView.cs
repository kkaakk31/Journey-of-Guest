using EditorAttributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoG.UI {

    public class TooltipView : MonoBehaviour {
        public TMP_Text tooltipText;
        public Image backgroundImage;
        public RectTransform rectTransform;
        public GameObject tooltipObject;

        public void Show() {
            tooltipObject.SetActive(true);
        }

        public void Hide() {
            tooltipObject.SetActive(false);
        }

        [Button]
        public void SetTooltip(string tooltip) {
            var textSize = tooltipText.GetPreferredValues(tooltip, 300f, 0);
            rectTransform.sizeDelta = textSize;
            tooltipText.text = tooltip;
        }

        [Button]
        public void SetBackgroundColor(in Color color) {
            backgroundImage.color = color;
        }

        public void SetPosition(in Vector2 position) {
            rectTransform.anchoredPosition = position;
        }

        private void Awake() {
            Hide();
        }
    }
}