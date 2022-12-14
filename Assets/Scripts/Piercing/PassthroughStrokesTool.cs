using UnityEngine;
using Utils;

namespace Piercing {
    public class PassthroughStrokesTool : MonoBehaviour, IBlendingTool {
        private static readonly Vector3 BRUSH_OFFSET = Vector3.forward * 0.1f;

        private Camera _camera;
        private Brush _leftBrush;
        private Brush _rightBrush;

        private void Awake() {
            _camera = Camera.main;
            _leftBrush = transform.FindChildGameObjectByName("LeftBrush").GetComponent<Brush>();
            _rightBrush = transform.FindChildGameObjectByName("RightBrush").GetComponent<Brush>();
        }

        private void Start() {
            OnDeselect();
        }

        private void Update() {
            _camera.depthTextureMode = DepthTextureMode.Depth;
            if (OVRInput.GetActiveController() is OVRInput.Controller.LTouch or OVRInput.Controller.RTouch
                or OVRInput.Controller.Touch) {
                _leftBrush.transform.position = GetLocalControllerPosition(OVRInput.Controller.LTouch);
                _rightBrush.transform.position = GetLocalControllerPosition(OVRInput.Controller.RTouch);
            }
        }

        private static Vector3 GetLocalControllerPosition(OVRInput.Controller controllerType) {
            return OVRInput.GetLocalControllerPosition(controllerType) +
                   OVRInput.GetLocalControllerRotation(controllerType) * BRUSH_OFFSET;
        }

        public void OnSelect() {
            gameObject.SetActive(true);
        }

        public void OnDeselect() {
            gameObject.SetActive(false);
        }
    }
}
