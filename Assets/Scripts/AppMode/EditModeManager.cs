using Fading;
using Piercing;
using UnityEngine;

namespace AppMode {
    public class EditModeManager : MonoBehaviour, IModeManager {
        [SerializeField] private PassthroughStrokesTool passthroughStrokesTool;
        [SerializeField] private PassthroughShapesTool passthroughShapesTool;
        [SerializeField] private FadingTool fadingTool;

        public void DisableAll() {
            passthroughStrokesTool.OnDeselect();
            passthroughShapesTool.OnDeselect();
            fadingTool.OnDeselect();
        }

        public void LockAll() {
            passthroughStrokesTool.Lock();
            passthroughShapesTool.Lock();
            fadingTool.Lock();
        }

        public void UnlockAll() {
            passthroughStrokesTool.Unlock();
            passthroughShapesTool.Unlock();
            fadingTool.Unlock();
        }

        public void ActivateStrokesTool() {
            passthroughShapesTool.OnDeselect();
            fadingTool.OnDeselect();
            passthroughStrokesTool.OnSelect();
        }

        public void ActivateShapesTool() {
            passthroughStrokesTool.OnDeselect();
            fadingTool.OnDeselect();
            passthroughShapesTool.OnSelect();
        }

        public void ActivateFadingTool() {
            passthroughStrokesTool.OnDeselect();
            passthroughShapesTool.OnDeselect();
            fadingTool.OnSelect();
        }
    }
}
