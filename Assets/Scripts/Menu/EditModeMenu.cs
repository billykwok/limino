using AppMode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu {
    public class EditModeMenu : MonoBehaviour, IMenu, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private MenuManager menuManager;
        [SerializeField] private FadingMenu fadingMenu;
        [SerializeField] private EditModeManager editModeManager;
        [SerializeField] private Button buttonBack;
        [SerializeField] private Button buttonStrokes;
        [SerializeField] private Button buttonShapes;
        [SerializeField] private Button buttonFading;

        private ActiveTool _activeTool = ActiveTool.None;

        private enum ActiveTool {
            Strokes,
            Shapes,
            Fading,
            None
        }

        public void Show() {
            buttonBack.onClick.AddListener(OnButtonBackClick);
            buttonStrokes.onClick.AddListener(OnButtonStrokesClick);
            buttonShapes.onClick.AddListener(OnButtonShapesClick);
            buttonFading.onClick.AddListener(OnButtonFadingClick);
            fadingMenu.Hide();
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
            _activeTool = ActiveTool.None;
        }

        private void OnButtonStrokesClick() {
            if (_activeTool == ActiveTool.Strokes) {
                editModeManager.DisableAll();
                _activeTool = ActiveTool.None;
            } else {
                fadingMenu.Hide();
                editModeManager.ActivateStrokesTool();
                _activeTool = ActiveTool.Strokes;
            }
        }

        private void OnButtonShapesClick() {
            if (_activeTool == ActiveTool.Shapes) {
                editModeManager.DisableAll();
                _activeTool = ActiveTool.None;
            } else {
                fadingMenu.Hide();
                editModeManager.ActivateShapesTool();
                _activeTool = ActiveTool.Shapes;
            }
        }

        private void OnButtonFadingClick() {
            if (_activeTool == ActiveTool.Fading) {
                editModeManager.DisableAll();
                _activeTool = ActiveTool.None;
            } else {
                fadingMenu.Show();
                editModeManager.ActivateFadingTool();
                _activeTool = ActiveTool.Fading;
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            editModeManager.LockAll();
        }

        public void OnPointerExit(PointerEventData eventData) {
            editModeManager.UnlockAll();
        }
    }
}
