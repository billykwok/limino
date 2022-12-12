using UnityEngine;
using Utils;

namespace Casting {
    public class FlashlightTool : MonoBehaviour, IBlendingTool {
        [SerializeField] private Transform torch;

        private static readonly Vector3 FLASHLIGHT_OFFSET = new(0, -0.03f, 0);

        private void Start() {
            gameObject.SetActive(false);
            torch.gameObject.SetActive(false);
        }

        private void LateUpdate() {
            var activeController = OVRInput.GetActiveController();
            if (activeController is OVRInput.Controller.RTouch or OVRInput.Controller.LTouch
                or OVRInput.Controller.Touch) {
                torch.SetPositionAndRotation(
                    OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch) + FLASHLIGHT_OFFSET,
                    OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch)
                );
            }
        }

        public void OnSelect() {
            torch.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        public void OnDeselect() {
            torch.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
