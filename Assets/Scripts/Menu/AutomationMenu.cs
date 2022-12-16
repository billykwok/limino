using AppMode;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
    public class AutomationMenu : MonoBehaviour, IMenu {
        [SerializeField] private MenuManager menuManager;
        [SerializeField] private AutomationManager automationManager;
        [SerializeField] private Toggle toggleBreakTime;
        [SerializeField] private Toggle toggleItemSearching;
        [SerializeField] private Toggle toggleBystander;

        private void Awake() {
            toggleBreakTime.isOn = false;
            toggleItemSearching.isOn = false;
            toggleBystander.isOn = false;
            toggleBreakTime.onValueChanged.AddListener(OnToggleBreakTime);
            toggleItemSearching.onValueChanged.AddListener(OnItemSearching);
            toggleBystander.onValueChanged.AddListener(OnBystander);
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
