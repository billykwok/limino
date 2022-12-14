using AppMode;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
    public class AutomationMenu : MonoBehaviour, IMenu {
        [SerializeField] private MenuManager menuManager;
        [SerializeField] private AutomationManager automationManager;
        [SerializeField] private Button buttonBack;
        [SerializeField] private Toggle toggleBreakTime;
        [SerializeField] private Toggle toggleItemSearching;
        [SerializeField] private Toggle toggleBystander;

        private void Awake() {
            toggleBreakTime.isOn = false;
            toggleItemSearching.isOn = false;
            toggleBystander.isOn = false;
        }

        public void Show() {
            buttonBack.onClick.AddListener(OnButtonBackClick);
            toggleBreakTime.onValueChanged.AddListener(OnToggleBreakTime);
            toggleItemSearching.onValueChanged.AddListener(OnItemSearching);
            toggleBystander.onValueChanged.AddListener(OnBystander);
            gameObject.SetActive(true);
        }

        public void Hide() {
            buttonBack.onClick.RemoveListener(OnButtonBackClick);
            toggleBreakTime.onValueChanged.RemoveListener(OnToggleBreakTime);
            toggleItemSearching.onValueChanged.RemoveListener(OnItemSearching);
            toggleItemSearching.onValueChanged.RemoveListener(OnBystander);
            gameObject.SetActive(false);
        }

        public bool IsShown() {
            return gameObject.activeSelf;
        }

        private void OnButtonBackClick() {
            menuManager.SwitchMode(MenuManager.MenuType.Play);
        }

        private void OnToggleBreakTime(bool isEnabled) {
            automationManager.ToggleBreakTimeDetection(isEnabled);
        }

        private void OnItemSearching(bool isEnabled) {
            automationManager.ToggleItemSearchingDetection(isEnabled);
        }

        private void OnBystander(bool isEnabled) {
            automationManager.ToggleBystanderDetection(isEnabled);
        }
    }
}
