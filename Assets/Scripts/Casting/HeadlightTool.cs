using UnityEngine;
using Utils;

namespace Casting {
    public class HeadlightTool : MonoBehaviour, IBlendingTool {
        [SerializeField] private Transform torch;

        private static readonly Vector3 HEADLIGHT_POSITION_OFFSET = new(0, -0.2f, 0);
        private static readonly Quaternion HEADLIGHT_ROTATION_OFFSET = Quaternion.Euler(20, 0, 0);

        private OVRCameraRig _cameraRig;

        private void Awake() {
            _cameraRig = FindObjectOfType<OVRCameraRig>();
        }

        private void Start() {
            gameObject.SetActive(false);
            torch.gameObject.SetActive(false);
        }

        private void LateUpdate() {
            torch.position = _cameraRig.centerEyeAnchor.position - HEADLIGHT_POSITION_OFFSET;
            torch.rotation = _cameraRig.centerEyeAnchor.rotation * HEADLIGHT_ROTATION_OFFSET;
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
