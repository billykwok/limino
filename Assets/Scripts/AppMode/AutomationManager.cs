using System;
using Casting;
using Fading;
using UnityEngine;

namespace AppMode {
    public class AutomationManager : MonoBehaviour {
        [SerializeField] private HeadlightTool headlightTool;
        [SerializeField] private FadingTool fadingTool;

        private static readonly float THRESHOLD_ITEM_SEARCHING = 0.3f;
        private static readonly float THRESHOLD_BREAK_TIME = 1.4f;

        private Camera _camera;
        private bool _isBreakTimeDetectionEnabled;
        private bool _isItemSearchingDetectionEnabled;
        private bool _isBystanderDetectionEnabled;
        private int _doorOpacity = 0;
        private Vector3 _lastLeftHandPosition = Vector3.zero;
        private Vector3 _lastRightHandPosition = Vector3.zero;
        private float _lastLeftHandDistance = Mathf.Infinity;
        private float _lastRightHandDistance = Mathf.Infinity;

        private void Awake() {
            _camera = Camera.main;
        }

        public void ToggleBreakTimeDetection(bool isEnabled) {
            _isBreakTimeDetectionEnabled = isEnabled;
        }

        public void ToggleItemSearchingDetection(bool isEnabled) {
            _isItemSearchingDetectionEnabled = isEnabled;
        }

        public void ToggleBystanderDetection(bool isEnabled) {
            _isBystanderDetectionEnabled = isEnabled;
        }

        private void Update() {
            var cameraPosition = _camera.transform.position;
            if (_isItemSearchingDetectionEnabled && OVRInput.GetActiveController() is OVRInput.Controller.LHand
                    or OVRInput.Controller.RHand or OVRInput.Controller.Hands) {
                var cameraHorizontalPosition = new Vector2(cameraPosition.x, cameraPosition.z);
                var leftHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand);
                var leftHandHorizontalPosition = new Vector2(leftHandPosition.x, leftHandPosition.z);
                var rightHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand);
                var rightHandHorizontalPosition = new Vector2(rightHandPosition.x, rightHandPosition.z);
                var leftDistance = Vector2.Distance(cameraHorizontalPosition, leftHandHorizontalPosition);
                var rightDistance = Vector2.Distance(cameraHorizontalPosition, rightHandHorizontalPosition);

                if ((leftDistance < 2f && leftDistance >= THRESHOLD_ITEM_SEARCHING &&
                     leftHandPosition.z - _lastLeftHandPosition.z < -0.015f &&
                     _lastLeftHandDistance < THRESHOLD_ITEM_SEARCHING) ||
                    (rightDistance < 2f && rightDistance >= THRESHOLD_ITEM_SEARCHING &&
                     rightHandPosition.z - _lastRightHandPosition.z < -0.015f &&
                     _lastRightHandDistance < THRESHOLD_ITEM_SEARCHING)) {
                    headlightTool.OnSelect();
                } else if ((leftDistance > 0 && leftDistance < THRESHOLD_ITEM_SEARCHING &&
                            _lastLeftHandPosition.z - leftHandPosition.z < -0.015f &&
                            _lastLeftHandDistance >= THRESHOLD_ITEM_SEARCHING) ||
                           (rightDistance > 0 && rightDistance < THRESHOLD_ITEM_SEARCHING &&
                            _lastRightHandPosition.z - rightHandPosition.z < -0.015f &&
                            _lastRightHandDistance >= THRESHOLD_ITEM_SEARCHING)) {
                    headlightTool.OnDeselect();
                }

                _lastLeftHandPosition = leftHandPosition;
                _lastRightHandPosition = rightHandPosition;
                _lastLeftHandDistance = leftDistance;
                _lastRightHandDistance = rightDistance;
            }

            if (_isBreakTimeDetectionEnabled) {
                fadingTool.OverrideGlobalOpacity(
                    Math.Clamp(
                        (cameraPosition.y - THRESHOLD_BREAK_TIME) * 8,
                        0,
                        1
                    )
                );
            }

            if (_isBystanderDetectionEnabled) {
                if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
                    _doorOpacity = 1 - _doorOpacity;
                    fadingTool.OverrideDoorOpacity(_doorOpacity);
                }
            }
        }
    }
}
