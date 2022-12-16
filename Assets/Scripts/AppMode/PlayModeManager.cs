using Casting;
using UnityEngine;

namespace AppMode {
    public class PlayModeManager : MonoBehaviour, IModeManager {
        [SerializeField] private FlashlightTool flashlightTool;
        [SerializeField] private HeadlightTool headlightTool;

        public void DisableAll() {
            flashlightTool.OnDeselect();
            headlightTool.OnDeselect();
        }

        public void LockAll() {
        }

        public void UnlockAll() {
        }

        public void ActivateFlashlightTool() {
            headlightTool.OnDeselect();
            flashlightTool.OnSelect();
        }

        public void ActivateHeadlightTool() {
            flashlightTool.OnDeselect();
            headlightTool.OnSelect();
        }
    }
}
