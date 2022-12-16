using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Menu {
    public class MenuManager : MonoBehaviour {
        public enum MenuType {
            Play,
            Edit
        }

        private readonly Dictionary<MenuType, IMenu> _menuManagersByMode = new();
        private MenuType _menuType = MenuType.Play;

        private void Awake() {
            _menuManagersByMode.Add(MenuType.Play, FindObjectOfType<PlayModeMenu>());
            _menuManagersByMode.Add(MenuType.Edit, FindObjectOfType<EditModeMenu>());
        }

        private void Start() {
            foreach (var menu in _menuManagersByMode.Values) {
                menu.Hide();
            }
        }

        private void Update() {
            var currentMenu = _menuManagersByMode.GetValueOrDefault(_menuType);
            if (currentMenu.IsShown()) {
                return;
            }

            currentMenu.Show();
            foreach (var menu in from mode in _menuManagersByMode.Keys
                     where mode != _menuType
                     select _menuManagersByMode[mode]
                     into menu
                     where menu.IsShown()
                     select menu) {
                menu.Hide();
            }
        }

        public bool IsHidden() {
            return !gameObject.activeSelf;
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        public void Summon(Vector3 menuPosition, Vector3 eyePosition) {
            transform.SetPositionAndRotation(menuPosition, Quaternion.LookRotation(menuPosition - eyePosition));
            gameObject.SetActive(true);
        }

        public void SwitchMode(MenuType menuType) {
            _menuType = menuType;
        }
    }
}
