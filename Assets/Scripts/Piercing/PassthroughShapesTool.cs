using System;
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
        private float _size;
        private float _distance = 0.5f;
        private readonly List<MeshRenderer> _shapes = new();

        private void Awake() {
            var ovrCameraRig = FindObjectOfType<OVRCameraRig>();
            ovrCameraRig.GetComponent<OVRPassthroughLayer>();
            _quadOutline = projectionObject.GetComponent<MeshRenderer>();
        }

        private void Start() {
            OnDeselect();
        }

        private void LateUpdate() {
            var thumbstickXY = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
            _size = thumbstickXY.x switch {
                >= 0.25f => Mathf.Clamp(_size + 0.01f, 0.1f, 0.5f),
                <= -0.25f => Mathf.Clamp(_size - 0.01f, 0.1f, 0.5f),
                _ => _size
            };
            transform.localScale = new Vector3(_size, _size, _size);
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
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, CONTROLLING_HAND)) {
                _shapes.Add(Instantiate(_quadOutline, container.transform, true));
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
