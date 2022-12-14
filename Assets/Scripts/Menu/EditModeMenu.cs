using AppMode;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
    public class EditModeMenu : MonoBehaviour, IMenu {
        [SerializeField] private MenuManager menuManager;
        [SerializeField] private EditModeManager editModeManager;
        [SerializeField] private Button buttonBack;
        [SerializeField] private Button buttonStrokes;
        [SerializeField] private Button buttonShapes;
        [SerializeField] private Button buttonFading;

        public void Show() {
            buttonBack.onClick.AddListener(OnButtonBackClick);
            buttonStrokes.onClick.AddListener(OnButtonStrokesClick);
            buttonShapes.onClick.AddListener(OnButtonShapesClick);
            buttonFading.onClick.AddListener(OnButtonFadingClick);
            gameObject.SetActive(true);
        }

        public void Hide() {
            buttonBack.onClick.RemoveListener(OnButtonBackClick);
            buttonStrokes.onClick.RemoveListener(OnButtonStrokesClick);
            buttonShapes.onClick.RemoveListener(OnButtonShapesClick);
            buttonShapes.onClick.RemoveListener(OnButtonFadingClick);
            gameObject.SetActive(false);
        }

        public bool IsShown() {
            return gameObject.activeSelf;
        }

        private void OnButtonBackClick() {
            editModeManager.DisableAll();
            menuManager.SwitchMode(MenuManager.MenuType.Play);
        }

        private void OnButtonStrokesClick() {
            editModeManager.ActivateStrokesTool();
        }

        private void OnButtonShapesClick() {
            editModeManager.ActivateShapesTool();
        }

        private void OnButtonFadingClick() {
            editModeManager.ActivateFadingTool();
        }
    }
}
