using System.Collections.Generic;
using UnityEngine;

public class LSPassthroughBrush : MonoBehaviour {
    public enum BrushState {
        Idle,
        Inking
    }

    public OVRInput.Controller controllerHand = OVRInput.Controller.None;
    public GameObject lineSegmentPrefab;
    public GameObject lineContainer;
    public bool forceActive = true;

    private BrushState brushStatus = BrushState.Idle;
    private LineRenderer currentLineSegment;
    private readonly List<Vector3> inkPositions = new();
    private readonly float minInkDist = 0.01f;
    private float strokeLength;
    private float strokeWidth = 0.1f;

    private void LateUpdate() {
        // face camera
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        if (!(controllerHand == OVRInput.Controller.LTouch || controllerHand == OVRInput.Controller.RTouch)) {
            return;
        }

        var tipPosition = transform.position;
        switch (brushStatus) {
            case BrushState.Idle:
                if (OVRInput.GetUp(OVRInput.Button.One, controllerHand)) {
                    UndoInkLine();
                }

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerHand)) {
                    StartLine(tipPosition);
                    brushStatus = BrushState.Inking;
                }

                break;

            case BrushState.Inking:
                // every frame, draw line
                UpdateLine(tipPosition);
                if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controllerHand)) {
                    brushStatus = BrushState.Idle;
                }

                break;
        }
    }

    private void OnDisable() {
        brushStatus = BrushState.Idle;
    }

    private void StartLine(Vector3 inkPos) {
        var newLine = Instantiate(lineSegmentPrefab, inkPos, Quaternion.identity);
        currentLineSegment = newLine.GetComponent<LineRenderer>();
        currentLineSegment.positionCount = 1;
        currentLineSegment.SetPosition(0, inkPos);
        strokeWidth = currentLineSegment.startWidth;
        strokeLength = 0.0f;
        inkPositions.Clear();
        inkPositions.Add(inkPos);
        newLine.transform.parent = lineContainer.transform;
    }

    private void UpdateLine(Vector3 inkPos) {
        var segmentLength = (inkPos - inkPositions[inkPositions.Count - 1]).magnitude;
        if (segmentLength >= minInkDist) {
            inkPositions.Add(inkPos);
            currentLineSegment.positionCount = inkPositions.Count;
            currentLineSegment.SetPositions(inkPositions.ToArray());
            strokeLength += segmentLength;
            // passing the line length to the shader ensures that the tail/end fades are consistent width
            currentLineSegment.material.SetFloat("_LineLength", strokeLength / strokeWidth);
        }
    }

    public void ClearLines() {
        for (var i = 0; i < lineContainer.transform.childCount; i++) {
            Destroy(lineContainer.transform.GetChild(i).gameObject);
        }
    }

    public void UndoInkLine() {
        if (lineContainer.transform.childCount >= 1) {
            Destroy(lineContainer.transform.GetChild(lineContainer.transform.childCount - 1).gameObject);
        }
    }
}
