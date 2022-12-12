using UnityEngine;
using Utils;

namespace Piercing {
    public class PassthroughShapesTool : MonoBehaviour, IBlendingTool {
        [SerializeField] private MeshFilter projectionObject;
        [SerializeField] private GameObject container;

        private const OVRInput.Controller CONTROLLING_HAND = OVRInput.Controller.RTouch;
        private static readonly Quaternion FACE_USER = Quaternion.Euler(-90, 0, 0);
        private static readonly Vector3 OFFSET = Vector3.forward * 0.5f;

        private MeshRenderer _quadOutline;

        private void Awake() {
            var ovrCameraRig = FindObjectOfType<OVRCameraRig>();
            ovrCameraRig.GetComponent<OVRPassthroughLayer>();
            _quadOutline = projectionObject.GetComponent<MeshRenderer>();
        }

        private void Start() {
            OnDeselect();
        }

        private void Update() {
            var rotation = OVRInput.GetLocalControllerRotation(CONTROLLING_HAND);
            _quadOutline.transform.SetPositionAndRotation(
                OVRInput.GetLocalControllerPosition(CONTROLLING_HAND) + rotation * OFFSET,
                rotation * FACE_USER
            );
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, CONTROLLING_HAND)) {
                Instantiate(_quadOutline, container.transform, true);
            }
        }

        public void OnSelect() {
            gameObject.SetActive(true);
        }

        public void OnDeselect() {
            gameObject.SetActive(false);
        }
    }
}
