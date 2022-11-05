using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OVRSceneAnchor))]
[DefaultExecutionOrder(30)]
public class ObjectSpawner : MonoBehaviour {
    private static GameObject _roomLightRef;

    public GameObject roomLight;
    public List<DynamicObject> dynamicObjects;

    private OVRSemanticClassification _classification;
    private int _frameCounter;
    private OVRSceneAnchor _sceneAnchor;

    private void Start() {
        _sceneAnchor = GetComponent<OVRSceneAnchor>();
        _classification = GetComponent<OVRSemanticClassification>();
        AddRoomLight();
        SpawnDynamicObject();
    }

    private void SpawnDynamicObject() {
        if (!FindValidDynamicObject(out var currentDynamicObject)) {
            return;
        }

        // Get current anchor's information
        var position = transform.position;
        var rotation = transform.rotation;

        var plane = _sceneAnchor.GetComponent<OVRScenePlane>();
        var volume = _sceneAnchor.GetComponent<OVRSceneVolume>();

        var dimensions = volume ? volume.Dimensions : Vector3.one;

        if (_classification && plane) {
            dimensions = plane.Dimensions;
            dimensions.z = 1;

            // Special case 01: Has only top plane
            if (_classification.Contains(OVRSceneManager.Classification.Desk) ||
                _classification.Contains(OVRSceneManager.Classification.Couch)) {
                GetVolumeFromTopPlane(
                    transform,
                    plane.Dimensions,
                    position.y,
                    out position,
                    out rotation,
                    out var localScale
                );

                dimensions = localScale;
                // The pivot for the resizer is at the top
                position.y += localScale.y / 2.0f;
            }

            // Special case 02: Set wall thickness to something small instead of default value (1.0m)
            if (_classification.Contains(OVRSceneManager.Classification.WallFace) ||
                _classification.Contains(OVRSceneManager.Classification.Ceiling) ||
                _classification.Contains(OVRSceneManager.Classification.Floor)) {
                dimensions.z = 0.01f;
            }
        }

        var root = new GameObject("Root");
        root.transform.parent = transform;
        root.transform.SetPositionAndRotation(position, rotation);

        var resizer = new Resizer();
        resizer.CreateResizedObject(dimensions, root, currentDynamicObject.resizable);
    }

    private bool FindValidDynamicObject(out DynamicObject currentDynamicObject) {
        currentDynamicObject = null;

        if (!_classification) {
            return false;
        }

        var sceneManager = FindObjectOfType<OVRSceneManager>();
        if (!sceneManager) {
            return false;
        }

        foreach (var dynamicObject in dynamicObjects) {
            if (_classification.Contains(dynamicObject.classificationLabel)) {
                currentDynamicObject = dynamicObject;
                return true;
            }
        }

        return false;
    }

    private void AddRoomLight() {
        if (!roomLight) {
            return;
        }

        if (_classification &&
            _classification.Contains(OVRSceneManager.Classification.Ceiling) &&
            !_roomLightRef) {
            _roomLightRef = Instantiate(roomLight, _sceneAnchor.transform);
        }
    }

    private static void GetVolumeFromTopPlane(
        Transform plane,
        Vector2 dimensions,
        float height,
        out Vector3 position,
        out Quaternion rotation,
        out Vector3 localScale) {
        var halfHeight = height / 2.0f;
        position = plane.position - Vector3.up * halfHeight;
        rotation = Quaternion.LookRotation(-plane.up, Vector3.up);
        localScale = new Vector3(dimensions.x, halfHeight * 2.0f, dimensions.y);
    }
}