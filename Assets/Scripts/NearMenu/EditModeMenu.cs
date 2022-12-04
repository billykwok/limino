using System.Collections.Generic;
using UnityEngine;

namespace NearMenu {
    public class EditModeMenu : MonoBehaviour {
        private readonly Dictionary<string, GameObject> _menuItems = new();

        private void Awake() {
            foreach (Transform child in transform) {
                _menuItems.Add(child.name, child.gameObject);
            }
        }
    }
}
