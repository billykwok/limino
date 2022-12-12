using Menu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace InputSystem {
    public class InputSystemManager : MonoBehaviour {
        [FormerlySerializedAs("nearMenuManager")] [SerializeField] private MenuManager menuManager;

        private static readonly Vector3 NEAR_MENU_OFFSET = Vector3.forward * 0.4f;

        private OVRCameraRig _cameraRig;
        private OVRInputModule _inputModule;

        private void Awake() {
            _cameraRig = FindObjectOfType<OVRCameraRig>();
            _inputModule = FindObjectOfType<OVRInputModule>();
        }

        private void Start() {
            menuManager.Hide();
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

            if (menuManager.IsHidden()) {
                menuManager.Summon(
                    controllerLocalPosition,
                    _cameraRig.centerEyeAnchor.transform.position
                );
            } else {
                menuManager.Hide();
            }
        }
    }
}
