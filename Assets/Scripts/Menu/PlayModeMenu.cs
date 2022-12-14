using AppMode;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
    public class PlayModeMenu : MonoBehaviour, IMenu {
        [SerializeField] private MenuManager menuManager;
        [SerializeField] private PlayModeManager playModeManager;
        [SerializeField] private Button buttonEditMode;
        [SerializeField] private Button buttonAutomation;
        [SerializeField] private Button buttonFlashlight;
        [SerializeField] private Button buttonHeadlight;

        public void Show() {
            buttonEditMode.onClick.AddListener(OnButtonEditModeClick);
            buttonAutomation.onClick.AddListener(OnButtonAutomationClick);
            buttonFlashlight.onClick.AddListener(OnButtonFlashlightClick);
            buttonHeadlight.onClick.AddListener(OnButtonHeadlightClick);
            gameObject.SetActive(true);
        }

        public void Hide() {
            buttonEditMode.onClick.RemoveListener(OnButtonEditModeClick);
            buttonFlashlight.onClick.RemoveListener(OnButtonFlashlightClick);
            buttonHeadlight.onClick.RemoveListener(OnButtonHeadlightClick);
            gameObject.SetActive(false);
        }

        public bool IsShown() {
            return gameObject.activeSelf;
        }

        private void OnButtonEditModeClick() {
            playModeManager.DisableAll();
            menuManager.SwitchMode(MenuManager.MenuType.Edit);
        }

        private void OnButtonAutomationClick() {
            playModeManager.DisableAll();
            menuManager.SwitchMode(MenuManager.MenuType.Automation);
        }

        private void OnButtonFlashlightClick() {
            playModeManager.ActivateFlashlightTool();
        }

        private void OnButtonHeadlightClick() {
            playModeManager.ActivateHeadlightTool();
        }
    }
}
