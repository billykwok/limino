using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(OVRSceneAnchor))]
[DefaultExecutionOrder(30)]
public class LSFurnitureSpawner : MonoBehaviour {
    private static readonly LSResizer resizer = new();

    private static readonly IReadOnlyList<string> volumeFromTopPlane = new List<string> {
        OVRSceneManager.Classification.Desk,
        OVRSceneManager.Classification.Couch
    };

    private static readonly IReadOnlyList<string> planeOnly = new List<string> {
        OVRSceneManager.Classification.WallFace,
        OVRSceneManager.Classification.Ceiling,
        OVRSceneManager.Classification.Floor
    };

    [SerializeField] private GameObject roomLightPrefab;
    [SerializeField] private List<LSSpawnable> spawnables;

    private OVRSemanticClassification _classification;
    private static GameObject _roomLightRef;
    private int _frameCounter;
    private OVRSceneAnchor _sceneAnchor;

    private void Start() {
        _sceneAnchor = GetComponent<OVRSceneAnchor>();
        _classification = GetComponent<OVRSemanticClassification>();
        AddRoomLight();
        SpawnSpawnable();
    }

    private void SpawnSpawnable() {
        if (!FindValidSpawnable(out var currentSpawnable)) {
            return;
        }

        // Get current anchor's information
        var clonedTransform = transform;
        var position = clonedTransform.position;
        var rotation = clonedTransform.rotation;

        var volume = _sceneAnchor.GetComponent<OVRSceneVolume>();
        var dimensions = volume ? volume.Dimensions : Vector3.one;

        var plane = _sceneAnchor.GetComponent<OVRScenePlane>();

        if (_classification && plane) {
            dimensions = plane.Dimensions;
            dimensions.z = 1;

            // Special case 01: Has only top plane
            if (_classification.Labels.Any(volumeFromTopPlane.Contains)) {
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
            if (_classification.Labels.Any(planeOnly.Contains)) {
                dimensions.z = 0.01f;
            }
        }

        resizer.CreateResizedObject(
            dimensions,
            new GameObject("Root") {
                transform = {
                    parent = transform,
                    position = position,
                    rotation = rotation
                }
            },
            currentSpawnable.resizable
        );
    }

    private bool FindValidSpawnable(out LSSpawnable currentSpawnable) {
        currentSpawnable = null;

        if (!_classification || !FindObjectOfType<OVRSceneManager>()) {
            return false;
        }

        foreach (var spawnable in spawnables.Where(it => _classification.Contains(it.classificationLabel))) {
            currentSpawnable = spawnable;
            return true;
        }

        return false;
    }

    private void AddRoomLight() {
        if (!roomLightPrefab) return;
        if (_classification && _classification.Contains(OVRSceneManager.Classification.Ceiling) &&
            !_roomLightRef) {
            _roomLightRef = Instantiate(roomLightPrefab, _sceneAnchor.transform);
        }
    }

    private static void GetVolumeFromTopPlane(
        Transform plane,
        Vector2 dimensions,
        float height,
        out Vector3 position,
        out Quaternion rotation,
        out Vector3 localScale) {
        position = plane.position - Vector3.up * height * 0.5f;
        rotation = Quaternion.LookRotation(-plane.up, Vector3.up);
        localScale = new Vector3(dimensions.x, height, dimensions.y);
    }
}
