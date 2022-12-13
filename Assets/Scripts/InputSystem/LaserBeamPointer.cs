using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace InputSystem {
    public class LaserBeamPointer : OVRCursor {
        private enum PointerBehavior {
            On,
            Off,
            OnWhenHitTarget
        }

        [SerializeField] private float maxLength = 10.0f;
        [SerializeField] private PointerBehavior pointerBehavior;

        private const OVRInput.Controller CONTROLLER = OVRInput.Controller.RTouch;

        private readonly RaycastHit[] _raycastBuffer = new RaycastHit[3];
        private GameObject _cursorVisual;
        private Vector3 _startPoint;
        private Vector3 _forward;
        private Vector3 _endPoint;
        private LineRenderer _lineRenderer;
        private GameObject _lastHitObject;
        private bool _hitTarget;
        private bool _restoreOnInputAcquired;

        public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal) {
            _startPoint = start;
            _endPoint = dest;
            _hitTarget = true;
        }

        public override void SetCursorRay(Transform t) {
            _startPoint = t.position;
            _forward = t.forward;
            _hitTarget = false;
        }

        private void Awake() {
            _cursorVisual = transform.FindChildGameObjectByName("Sphere");
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start() {
            if (_cursorVisual) {
                _cursorVisual.SetActive(false);
            }

            OVRManager.InputFocusAcquired += OnInputFocusAcquired;
            OVRManager.InputFocusLost += OnInputFocusLost;
        }

        private void LateUpdate() {
            var controllerPos = OVRInput.GetLocalControllerPosition(CONTROLLER);
            var controllerRot = OVRInput.GetLocalControllerRotation(CONTROLLER);
            UpdateHoverObject(controllerPos, controllerRot);
            _lineRenderer.SetPosition(0, _startPoint);
            if (_hitTarget) {
                _lineRenderer.SetPosition(1, _endPoint);
                UpdateLaserBeam(_startPoint, _endPoint);
                if (_cursorVisual) {
                    _cursorVisual.transform.position = _endPoint;
                    _cursorVisual.SetActive(true);
                }
            } else {
                UpdateLaserBeam(_startPoint, _startPoint + maxLength * _forward);
                _lineRenderer.SetPosition(1, _startPoint + maxLength * _forward);
                if (_cursorVisual) {
                    _cursorVisual.SetActive(false);
                }
            }
        }

        private void OnDisable() {
            if (_cursorVisual) {
                _cursorVisual.SetActive(false);
            }
        }

        private void OnDestroy() {
            OVRManager.InputFocusAcquired -= OnInputFocusAcquired;
            OVRManager.InputFocusLost -= OnInputFocusLost;
        }

        private void UpdateLaserBeam(Vector3 start, Vector3 end) {
            switch (pointerBehavior) {
                case PointerBehavior.On: {
                    _lineRenderer.SetPosition(0, start);
                    _lineRenderer.SetPosition(1, end);
                    break;
                }

                case PointerBehavior.OnWhenHitTarget when _hitTarget: {
                    if (!_lineRenderer.enabled) {
                        _lineRenderer.SetPosition(0, start);
                        _lineRenderer.SetPosition(1, end);
                        _lineRenderer.enabled = true;
                    }

                    break;
                }

                case PointerBehavior.OnWhenHitTarget: {
                    if (_lineRenderer.enabled) {
                        _lineRenderer.enabled = false;
                    }

                    break;
                }
            }
        }

        private void UpdateHoverObject(Vector3 position, Quaternion rotation) {
            var objectHitCount = Physics.RaycastNonAlloc(
                position,
                rotation * Vector3.forward,
                _raycastBuffer
            );

            var closestDistance = Mathf.Infinity;
            GameObject currentlyHitObject = null;

            for (var i = 0; i < objectHitCount; ++i) {
                var hit = _raycastBuffer[i];
                var hitDistance = Vector3.Distance(hit.point, position);
                if (hitDistance < closestDistance) {
                    closestDistance = hitDistance;
                    currentlyHitObject = hit.transform.gameObject;
                }
            }

            _lastHitObject = currentlyHitObject;
        }

        private void OnInputFocusLost() {
            if (!gameObject || !gameObject.activeInHierarchy) {
                return;
            }

            _restoreOnInputAcquired = true;
            gameObject.SetActive(false);
        }

        private void OnInputFocusAcquired() {
            if (!_restoreOnInputAcquired || !gameObject) {
                return;
            }

            _restoreOnInputAcquired = false;
            gameObject.SetActive(true);
        }
    }
}
