using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Piercing {
    public class PassthroughShapesTool : MonoBehaviour, IBlendingTool {
        [SerializeField] private MeshFilter projectionObject;
        [SerializeField] private GameObject container;

        private const OVRInput.Controller CONTROLLING_HAND = OVRInput.Controller.RTouch;
        private static readonly Quaternion FACE_USER = Quaternion.Euler(0, 0, 0);

        private MeshRenderer _quadOutline;
        private float _size = 1f;
        private float _distance = 0.5f;
        private int _layerDefault;
        private bool _locked;
        private readonly List<MeshRenderer> _shapes = new();

        private void Awake() {
            var ovrCameraRig = FindObjectOfType<OVRCameraRig>();
            ovrCameraRig.GetComponent<OVRPassthroughLayer>();
            _quadOutline = projectionObject.GetComponent<MeshRenderer>();
            _layerDefault = LayerMask.NameToLayer("Default");
        }

        private void Start() {
            OnDeselect();
        }

        public void Lock() {
            _locked = true;
        }

        public void Unlock() {
            _locked = false;
        }

        private void LateUpdate() {
            var thumbstickXY = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
            _size = thumbstickXY.x switch {
                >= 0.25f => Mathf.Clamp(_size + 0.01f, 0.2f, 4f),
                <= -0.25f => Mathf.Clamp(_size - 0.01f, 0.2f, 4f),
                _ => _size
            };
            _quadOutline.transform.localScale = new Vector3(_size, _size, 0.00001f);
            _distance = thumbstickXY.y switch {
                >= 0.25f => Mathf.Clamp(_distance + 0.02f, 0.1f, 5f),
                <= -0.25f => Mathf.Clamp(_distance - 0.02f, 0.1f, 5f),
                _ => _distance
            };

            var rotation = OVRInput.GetLocalControllerRotation(CONTROLLING_HAND);
            _quadOutline.transform.SetPositionAndRotation(
                OVRInput.GetLocalControllerPosition(CONTROLLING_HAND) + rotation * (Vector3.forward * _distance),
                rotation * FACE_USER
            );
            if (_shapes.Count > 0 && OVRInput.GetUp(OVRInput.Button.One, CONTROLLING_HAND)) {
                var lastIndex = _shapes.Count - 1;
                Destroy(_shapes[lastIndex]);
                _shapes.RemoveAt(lastIndex);
            }

            if (!_locked && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, CONTROLLING_HAND)) {
                var meshRenderer = Instantiate(_quadOutline, container.transform, true);
                meshRenderer.gameObject.layer = _layerDefault;
                _shapes.Add(meshRenderer);
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
