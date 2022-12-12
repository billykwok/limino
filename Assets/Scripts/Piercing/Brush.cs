using System.Collections.Generic;
using UnityEngine;

namespace Piercing {
    public class Brush : MonoBehaviour {
        public OVRInput.Controller controllerHand = OVRInput.Controller.None;
        public GameObject lineSegmentPrefab;
        public GameObject strokesContainer;

        private enum BrushState {
            Idle,
            Inking
        }

        private const float MIN_INK_DIST = 0.01f;
        private static readonly int LINE_LENGTH = Shader.PropertyToID("_LineLength");

        private readonly List<Vector3> _inkPositions = new();

        private BrushState _brushStatus = BrushState.Idle;
        private LineRenderer _currentLineSegment;
        private float _strokeLength;
        private float _strokeWidth = 0.1f;
        private Camera _camera;

        private void Start() {
            _camera = Camera.main;
        }

        private void LateUpdate() {
            // face camera
            transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);

            if (controllerHand is not (OVRInput.Controller.LTouch or OVRInput.Controller.RTouch)) {
                return;
            }

            var tipPosition = transform.position;
            switch (_brushStatus) {
                case BrushState.Idle:
                    if (OVRInput.GetUp(OVRInput.Button.One, controllerHand)) {
                        UndoInkLine();
                    }

                    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerHand)) {
                        var newLine = Instantiate(lineSegmentPrefab, tipPosition, Quaternion.identity);
                        _currentLineSegment = newLine.GetComponent<LineRenderer>();
                        _currentLineSegment.positionCount = 1;
                        _currentLineSegment.SetPosition(0, tipPosition);
                        _strokeWidth = _currentLineSegment.startWidth;
                        _strokeLength = 0.0f;
                        _inkPositions.Clear();
                        _inkPositions.Add(tipPosition);
                        newLine.transform.parent = strokesContainer.transform;
                        _brushStatus = BrushState.Inking;
                    }

                    break;

                case BrushState.Inking:
                    var segmentLength = (tipPosition - _inkPositions[^1]).magnitude;
                    if (segmentLength < MIN_INK_DIST) {
                    } else {
                        _inkPositions.Add(tipPosition);
                        _currentLineSegment.positionCount = _inkPositions.Count;
                        _currentLineSegment.SetPositions(_inkPositions.ToArray());
                        _strokeLength += segmentLength;
                        // passing the line length to the shader ensures that the tail/end fades are consistent width
                        _currentLineSegment.material.SetFloat(LINE_LENGTH, _strokeLength / _strokeWidth);
                    }

                    if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controllerHand)) {
                        _brushStatus = BrushState.Idle;
                    }

                    break;
            }
        }

        private void OnDisable() {
            _brushStatus = BrushState.Idle;
        }

        private void ClearAllLines() {
            for (var i = 0; i < strokesContainer.transform.childCount; i++) {
                Destroy(strokesContainer.transform.GetChild(i).gameObject);
            }
        }

        private void UndoInkLine() {
            if (strokesContainer.transform.childCount >= 1) {
                Destroy(strokesContainer.transform.GetChild(strokesContainer.transform.childCount - 1).gameObject);
            }
        }
    }
}
