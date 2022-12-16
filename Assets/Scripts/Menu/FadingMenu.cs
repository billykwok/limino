using Fading;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
    public class FadingMenu : MonoBehaviour, IMenu {
        [SerializeField] private FadingTool fadingTool;
        [SerializeField] private Toggle togglePerObjectFading;

        private void Awake() {
            togglePerObjectFading.isOn = true;
            togglePerObjectFading.onValueChanged.AddListener(OnTogglePerObjectFading);
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        public bool IsShown() {
            return gameObject.activeSelf;
        }

        private void OnTogglePerObjectFading(bool isEnabled) {
            fadingTool.SwitchMode(isEnabled ? FadingTool.Mode.Object : FadingTool.Mode.Global);
        }
    }
}
