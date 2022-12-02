using UnityEngine;
using UnityEngine.Serialization;

namespace PassthroughStrokes {
    public class PassthroughStrokesManager : MonoBehaviour {
        private Camera _camera;
        private GameObject _leftBrush;
        private GameObject _rightBrush;

        private void Start() {
            _camera = Camera.main;
            _leftBrush = transform.Find("LeftBrush").gameObject;
            _rightBrush = transform.Find("RightBrush").gameObject;
        }

        private void Update() {
            _camera.depthTextureMode = DepthTextureMode.Depth;

            switch (OVRInput.GetActiveController()) {
                case OVRInput.Controller.LTouch:
                case OVRInput.Controller.RTouch:
                case OVRInput.Controller.Touch:
                    _leftBrush.SetActive(true);
                    _rightBrush.SetActive(true);
                    _leftBrush.transform.position = GetLocalControllerPosition(OVRInput.Controller.LTouch);
                    _rightBrush.transform.position = GetLocalControllerPosition(OVRInput.Controller.RTouch);
                    break;

                default:
                    _leftBrush.SetActive(false);
                    _rightBrush.SetActive(false);
                    break;
            }
        }

        private static Vector3 GetLocalControllerPosition(OVRInput.Controller controllerType) {
            return OVRInput.GetLocalControllerPosition(controllerType) +
                   OVRInput.GetLocalControllerRotation(controllerType) *
                   Vector3.forward * 0.1f;
        }
    }
}
