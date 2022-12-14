using System;
using System.Collections.Generic;
using UnityEngine;

namespace Piercing {
    public class Brush : MonoBehaviour {
        [SerializeField] private OVRInput.Controller controllerHand = OVRInput.Controller.None;
        [SerializeField] private GameObject lineSegmentPrefab;
        [SerializeField] private GameObject strokesContainer;

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
        private float _localDistance;
        private Camera _camera;
        private int _layerDefault;

        private void Awake() {
            _layerDefault = LayerMask.NameToLayer("Default");
        }

        private void Start() {
            _camera = Camera.main;
        }

        private void LateUpdate() {
            // face camera
            transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);

            if (controllerHand is not (OVRInput.Controller.LTouch or OVRInput.Controller.RTouch)) {
                return;
            }

            var thumbstickXY = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controllerHand);
            var controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
            _strokeWidth = thumbstickXY.x switch {
                >= 0.25f => Mathf.Clamp(_strokeWidth + 0.01f, 0.1f, 0.5f),
                <= -0.25f => Mathf.Clamp(_strokeWidth - 0.01f, 0.1f, 0.5f),
                _ => _strokeWidth
            };
            var brushScale = _strokeWidth * 5;
            transform.localScale = new Vector3(brushScale, brushScale, brushScale);
            _localDistance = thumbstickXY.y switch {
                >= 0.25f => Mathf.Clamp(_localDistance + 0.02f, 0.1f, 5f),
                <= -0.25f => Mathf.Clamp(_localDistance - 0.02f, 0.1f, 5f),
                _ => _localDistance
            };

            var tx = transform;
            tx.localPosition += controllerRotation * (Vector3.forward * _localDistance);
            var tipPosition = tx.position;
            switch (_brushStatus) {
                case BrushState.Idle:
                    if (OVRInput.GetUp(OVRInput.Button.One, controllerHand)) {
                        UndoInkLine();
                    }

                    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerHand)) {
                        var newLine = Instantiate(lineSegmentPrefab, tipPosition, Quaternion.identity);
                        newLine.layer = _layerDefault;
                        _currentLineSegment = newLine.GetComponent<LineRenderer>();
                        _currentLineSegment.positionCount = 1;
                        _currentLineSegment.SetPosition(0, tipPosition);
                        _currentLineSegment.startWidth = _strokeWidth;
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
                        _currentLineSegment.material.SetFloat(
                            LINE_LENGTH,
                            _strokeLength / _strokeWidth
                        );
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
