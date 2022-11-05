using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class Resizer {
    public void CreateResizedObject(Vector3 newSize, GameObject parent, Resizable sourcePrefab) {
        var prefab = Object.Instantiate(sourcePrefab.gameObject, Vector3.zero, Quaternion.identity);
        prefab.name = sourcePrefab.name;

        var resizable = prefab.GetComponent<Resizable>();
        resizable.newSize = newSize;
        if (resizable == null) {
            Debug.LogError("Resizable component missing.");
            return;
        }

        var resizedMesh = ProcessVertices(resizable, newSize);

        var mf = prefab.GetComponent<MeshFilter>();
        mf.sharedMesh = resizedMesh;
        mf.sharedMesh.RecalculateBounds();

        // child it after creation so the bounds math plays nicely
        prefab.transform.parent = parent.transform;
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;

        // cleanup
        Object.Destroy(resizable);
    }

    private Mesh ProcessVertices(Resizable resizable, Vector3 newSize) {
        var originalMesh = resizable.mesh;
        var originalBounds = originalMesh.bounds.size;

        // Force scaling if newSize is smaller than the original mesh
        var methodX = originalBounds.x < newSize.x
            ? resizable.scalingX
            : Resizable.Method.Scale;
        var methodY = originalBounds.y < newSize.y
            ? resizable.scalingY
            : Resizable.Method.Scale;
        var methodZ = originalBounds.z < newSize.z
            ? resizable.scalingZ
            : Resizable.Method.Scale;

        var resizedVertices = originalMesh.vertices;

        var pivotX = 1 / resizable.defaultSize.x * resizable.pivotPosition.x;
        var pivotY = 1 / resizable.defaultSize.y * resizable.pivotPosition.y;
        var pivotZ = 1 / resizable.defaultSize.z * resizable.pivotPosition.z;

        for (var i = 0; i < resizedVertices.Length; i++) {
            var vertexPosition = resizedVertices[i];
            vertexPosition.x = CalculateNewVertexPosition(
                methodX,
                vertexPosition.x,
                originalBounds.x,
                newSize.x,
                resizable.paddingX,
                resizable.paddingXMax,
                pivotX
            );

            vertexPosition.y = CalculateNewVertexPosition(
                methodY,
                vertexPosition.y,
                originalBounds.y,
                newSize.y,
                resizable.paddingY,
                resizable.paddingYMax,
                pivotY
            );

            vertexPosition.z = CalculateNewVertexPosition(
                methodZ,
                vertexPosition.z,
                originalBounds.z,
                newSize.z,
                resizable.paddingZ,
                resizable.paddingZMax,
                pivotZ
            );
            resizedVertices[i] = vertexPosition;
        }

        var clonedMesh = Object.Instantiate(originalMesh);
        clonedMesh.vertices = resizedVertices;

        return clonedMesh;
    }

    private float CalculateNewVertexPosition(
        Resizable.Method resizeMethod,
        float currentPosition,
        float currentSize,
        float newSize,
        float padding,
        float paddingMax,
        float pivot) {
        var resizedRatio = currentSize / 2
                           * (newSize / 2 * (1 / (currentSize / 2)))
                           - currentSize / 2;

        switch (resizeMethod) {
            case Resizable.Method.Adapt:
                if (Mathf.Abs(currentPosition) >= padding) {
                    currentPosition = resizedRatio * Mathf.Sign(currentPosition) + currentPosition;
                }

                break;

            case Resizable.Method.AdaptWithAsymmetricalPadding:
                if (currentPosition >= padding) {
                    currentPosition = resizedRatio * Mathf.Sign(currentPosition) + currentPosition;
                }

                if (currentPosition <= paddingMax) {
                    currentPosition = resizedRatio * Mathf.Sign(currentPosition) + currentPosition;
                }

                break;

            case Resizable.Method.Scale:
                currentPosition = newSize / (currentSize / currentPosition);
                break;

            case Resizable.Method.None:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(resizeMethod), resizeMethod, null);
        }

        var pivotPos = newSize * -pivot;
        currentPosition += pivotPos;

        return currentPosition;
    }
}