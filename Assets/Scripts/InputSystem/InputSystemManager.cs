using NearMenu;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputSystem {
    public class InputSystemManager : MonoBehaviour {
        [SerializeField] private NearMenuManager nearMenuManager;
        [SerializeField] private LineRenderer laser;

        private static readonly Vector3 NEAR_MENU_OFFSET = Vector3.forward * 0.4f;

        private OVRCameraRig _cameraRig;
        private OVRInputModule _inputModule;
        private Vector3 _cursorPosition = Vector3.zero;

        private void Awake() {
            _cameraRig = FindObjectOfType<OVRCameraRig>();
            _inputModule = FindObjectOfType<OVRInputModule>();
        }

        private void Start() {
            nearMenuManager.Hide();
        }

        private void Update() {
            var controller = OVRInput.GetActiveController() == OVRInput.Controller.LTouch
                ? OVRInput.Controller.LTouch
                : OVRInput.Controller.RTouch;
            _inputModule.rayTransform =
                controller == OVRInput.Controller.LTouch ? _cameraRig.leftHandAnchor : _cameraRig.rightHandAnchor;
        }

        private void LateUpdate() {
            Vector3 controllerLocalPosition;
            if (OVRInput.GetUp(OVRInput.Button.Start, OVRInput.Controller.LTouch)) {
                controllerLocalPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) +
                                          OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) *
                                          NEAR_MENU_OFFSET;
            } else if (OVRInput.GetUp(OVRInput.Button.Start, OVRInput.Controller.RTouch)) {
                controllerLocalPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch) +
                                          OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) *
                                          NEAR_MENU_OFFSET;
            } else {
                return;
            }

            if (nearMenuManager.IsHidden()) {
                nearMenuManager.Summon(
                    controllerLocalPosition,
                    _cameraRig.centerEyeAnchor.transform.position
                );
            } else {
                nearMenuManager.Hide();
            }
        }
    }
}
