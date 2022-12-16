using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu {
    public class TextRevealingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private TextMeshProUGUI _label;

        private void Awake() {
            _label = GetComponentInChildren<TextMeshProUGUI>();
            _label.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            _label.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData) {
            _label.gameObject.SetActive(false);
        }

        private void OnBecameInvisible() {
            _label.gameObject.SetActive(false);
        }
    }
}
