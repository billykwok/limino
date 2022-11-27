using UnityEngine;

namespace InputSystem {
    public class LaserBeamPointer : OVRCursor {
        private enum LaserBeamBehavior {
            On,
            Off,
            OnWhenHitTarget
        }

        [SerializeField] private GameObject cursorVisual;
        [SerializeField] private float maxLength = 10.0f;
        [SerializeField] private LaserBeamBehavior laserBeamBehavior;

        private Vector3 _startPoint;
        private Vector3 _forward;
        private Vector3 _endPoint;
        private LineRenderer _lineRenderer;
        private bool _hitTarget;
        private bool _restoreOnInputAcquired;

        private void Awake() {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start() {
            if (cursorVisual) {
                cursorVisual.SetActive(false);
            }

            OVRManager.InputFocusAcquired += OnInputFocusAcquired;
            OVRManager.InputFocusLost += OnInputFocusLost;
        }

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

        private void LateUpdate() {
            _lineRenderer.SetPosition(0, _startPoint);
            if (_hitTarget) {
                _lineRenderer.SetPosition(1, _endPoint);
                UpdateLaserBeam(_startPoint, _endPoint);
                if (cursorVisual) {
                    cursorVisual.transform.position = _endPoint;
                    cursorVisual.SetActive(true);
                }
            } else {
                UpdateLaserBeam(_startPoint, _startPoint + maxLength * _forward);
                _lineRenderer.SetPosition(1, _startPoint + maxLength * _forward);
                if (cursorVisual) {
                    cursorVisual.SetActive(false);
                }
            }
        }

        // make laser beam a behavior with a prop that enables or disables
        private void UpdateLaserBeam(Vector3 start, Vector3 end) {
            switch (laserBeamBehavior) {
                case LaserBeamBehavior.Off:
                    break;

                case LaserBeamBehavior.On:
                    _lineRenderer.SetPosition(0, start);
                    _lineRenderer.SetPosition(1, end);
                    break;

                case LaserBeamBehavior.OnWhenHitTarget when _hitTarget: {
                    if (!_lineRenderer.enabled) {
                        _lineRenderer.SetPosition(0, start);
                        _lineRenderer.SetPosition(1, end);
                        _lineRenderer.enabled = true;
                    }

                    break;
                }

                case LaserBeamBehavior.OnWhenHitTarget: {
                    if (_lineRenderer.enabled) {
                        _lineRenderer.enabled = false;
                    }

                    break;
                }
            }
        }

        private void OnDisable() {
            if (cursorVisual) {
                cursorVisual.SetActive(false);
            }
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

        private void OnDestroy() {
            OVRManager.InputFocusAcquired -= OnInputFocusAcquired;
            OVRManager.InputFocusLost -= OnInputFocusLost;
        }
    }
}
