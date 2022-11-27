using NearMenu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace InputSystem {
    public class InputSystemManager : MonoBehaviour {
        [SerializeField] private NearMenuManager nearMenuManager;

        private static readonly Vector3 NEAR_MENU_OFFSET = Vector3.forward * 0.4f;

        private OVRCameraRig _cameraRig;
        private OVRInputModule _inputModule;

        private void Start() {
            _cameraRig = FindObjectOfType<OVRCameraRig>();
            _inputModule = FindObjectOfType<OVRInputModule>();
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
            if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.LTouch)) {
                controllerLocalPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) +
                                          OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) *
                                          NEAR_MENU_OFFSET;
            } else if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch)) {
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