using AppMode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu {
    public class PlayModeMenu : MonoBehaviour, IMenu, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private MenuManager menuManager;
        [SerializeField] private PlayModeManager playModeManager;
        [SerializeField] private AutomationMenu automationMenu;
        [SerializeField] private Button buttonEditMode;
        [SerializeField] private Button buttonAutomation;
        [SerializeField] private Button buttonFlashlight;
        [SerializeField] private Button buttonHeadlight;

        private ActiveTool _activeTool = ActiveTool.None;

        private enum ActiveTool {
            Flashlight,
            Headlight,
            Automation,
            None
        }

        public void Show() {
            buttonEditMode.onClick.AddListener(OnButtonEditModeClick);
            buttonAutomation.onClick.AddListener(OnButtonAutomationClick);
            buttonFlashlight.onClick.AddListener(OnButtonFlashlightClick);
            buttonHeadlight.onClick.AddListener(OnButtonHeadlightClick);
            automationMenu.Hide();
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
            if (_activeTool == ActiveTool.Automation) {
                playModeManager.DisableAll();
                _activeTool = ActiveTool.None;
            } else {
                automationMenu.Show();
                _activeTool = ActiveTool.Automation;
            }
        }

        private void OnButtonFlashlightClick() {
            if (_activeTool == ActiveTool.Flashlight) {
                playModeManager.DisableAll();
                _activeTool = ActiveTool.None;
            } else {
                automationMenu.Hide();
                playModeManager.ActivateFlashlightTool();
                _activeTool = ActiveTool.Flashlight;
            }
        }

        private void OnButtonHeadlightClick() {
            if (_activeTool == ActiveTool.Headlight) {
                playModeManager.DisableAll();
                _activeTool = ActiveTool.None;
            } else {
                automationMenu.Hide();
                playModeManager.ActivateHeadlightTool();
                _activeTool = ActiveTool.Headlight;
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            playModeManager.LockAll();
        }

        public void OnPointerExit(PointerEventData eventData) {
            playModeManager.UnlockAll();
        }
    }
}
