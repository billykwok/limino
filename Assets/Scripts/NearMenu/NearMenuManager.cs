using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace NearMenu {
    public class NearMenuManager : MonoBehaviour {
        private enum Mode {
            Main,
            Edit
        }

        private readonly Dictionary<Mode, GameObject> _menusByMode = new();
        private Mode _mode = Mode.Main;

        private void Awake() {
            _menusByMode.Add(Mode.Main, transform.FindChildGameObjectByName("MainMenuCanvas"));
            _menusByMode.Add(Mode.Edit, transform.FindChildGameObjectByName("EditModeMenuCanvas"));
        }

        private void Start() {
            foreach (var menu in _menusByMode.Values) {
                menu.SetActive(false);
            }
        }

        private void Update() {
            var currentMenu = _menusByMode.GetValueOrDefault(_mode);
            if (currentMenu.activeSelf) {
                return;
            }

            currentMenu.SetActive(true);
            foreach (var menu in from mode in _menusByMode.Keys
                     where mode != _mode
                     select _menusByMode[mode]
                     into menu
                     where menu.activeSelf
                     select menu) {
                menu.SetActive(false);
            }
        }

        public bool IsHidden() {
            return !gameObject.activeSelf;
        }

        public void EnterEditMode() {
            _mode = Mode.Edit;
        }

        public void ExitEditMode() {
            _mode = Mode.Main;
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        public void Summon(Vector3 menuPosition, Vector3 eyePosition) {
            transform.SetPositionAndRotation(menuPosition, Quaternion.LookRotation(menuPosition - eyePosition));
            gameObject.SetActive(true);
        }
    }
}
