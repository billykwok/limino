using UnityEngine;

namespace PassthroughShadow {
    public class PassthroughShadowManager : MonoBehaviour {
        private enum Mode {
            Hand,
            Head
        }

        public Light sceneLight;
        public Transform flashlightRoot;

        private OVRCameraRig _cameraRig;
        private Vector3 _localPosition = Vector3.zero;
        private Quaternion _localRotation = Quaternion.identity;
        private Mode _mode = Mode.Hand;
        private Flashlight _flashLight;

        private void Awake() {
            _cameraRig = FindObjectOfType<OVRCameraRig>();
            _flashLight = GetComponent<Flashlight>();
        }

        private void Start() {
            _localRotation = flashlightRoot.localRotation;
            _localPosition = flashlightRoot.localPosition;
            gameObject.SetActive(false);
        }

        private void LateUpdate() {
            if (_mode == Mode.Hand) {
                if (OVRInput.GetActiveController() == OVRInput.Controller.RTouch ||
                    OVRInput.GetActiveController() == OVRInput.Controller.LTouch ||
                    OVRInput.GetActiveController() == OVRInput.Controller.Touch) {
                    AlignWithController(OVRInput.Controller.RTouch);
                }
            } else {
                AlignWithHead();
            }
        }

        public void ActivateForHand() {
            if (gameObject.activeSelf && _mode == Mode.Hand) {
                gameObject.SetActive(false);
            } else {
                _mode = Mode.Hand;
                gameObject.SetActive(true);
            }
        }

        public void ActivateForHead() {
            if (gameObject.activeSelf && _mode == Mode.Head) {
                gameObject.SetActive(false);
            } else {
                _mode = Mode.Head;
                gameObject.SetActive(true);
            }
        }

        private void AlignWithController(OVRInput.Controller controller) {
            transform.position = OVRInput.GetLocalControllerPosition(controller);
            transform.rotation = OVRInput.GetLocalControllerRotation(controller);
            flashlightRoot.localRotation = _localRotation;
            flashlightRoot.localPosition = _localPosition;
        }

        private void AlignWithHead() {
            var transformCopy = transform;
            transformCopy.position = _cameraRig.centerEyeAnchor.position - new Vector3(0, -0.2f, 0);
            transformCopy.rotation = _cameraRig.centerEyeAnchor.rotation * Quaternion.Euler(30, 0, 0);
            flashlightRoot.localRotation = _localRotation;
            flashlightRoot.localPosition = _localPosition;
        }
    }
}
