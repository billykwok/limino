using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Menu {
    public class MenuManager : MonoBehaviour {
        public enum Mode {
            Play,
            Edit
        }

        private readonly Dictionary<Mode, IMenu> _menuManagersByMode = new();
        private Mode _mode = Mode.Play;

        private void Awake() {
            _menuManagersByMode.Add(Mode.Play, FindObjectOfType<PlayModeMenu>());
            _menuManagersByMode.Add(Mode.Edit, FindObjectOfType<EditModeMenu>());
        }

        private void Start() {
            foreach (var menu in _menuManagersByMode.Values) {
                menu.Hide();
            }
        }

        private void Update() {
            var currentMenu = _menuManagersByMode.GetValueOrDefault(_mode);
            if (currentMenu.IsShown()) {
                return;
            }

            currentMenu.Show();
            foreach (var menu in from mode in _menuManagersByMode.Keys
                     where mode != _mode
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

        public void SwitchMode(Mode mode) {
            _mode = mode;
        }
    }
}
