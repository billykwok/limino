using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class LSResizer {
    public void CreateResizedObject(Vector3 newSize, GameObject parent, LSResizable sourcePrefab) {
        var prefab = Object.Instantiate(sourcePrefab.gameObject, Vector3.zero, Quaternion.identity);
        prefab.name = sourcePrefab.name;

        var resizable = prefab.GetComponent<LSResizable>();
        if (resizable is null) {
            Debug.LogError("Resizable component missing.");
            return;
        }

        resizable.newSize = newSize;

        var resizedMesh = ProcessVertices(resizable, newSize);

        var mf = prefab.GetComponent<MeshFilter>();
        mf.sharedMesh = resizedMesh;
        mf.sharedMesh.RecalculateBounds();

        var boxCollider = prefab.GetComponent<BoxCollider>();
        if (boxCollider is not null) {
            boxCollider.size = mf.sharedMesh.bounds.size;
        }

        // child it after creation so the bounds math plays nicely
        prefab.transform.SetParent(parent.transform);
        prefab.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        // cleanup
        Object.Destroy(resizable);
    }

    private Mesh ProcessVertices(LSResizable resizable, Vector3 newSize) {
        var originalMesh = resizable.mesh;
        var originalBounds = originalMesh.bounds.size;

        // Force scaling if newSize is smaller than the original mesh
        var methodX = originalBounds.x < newSize.x
            ? resizable.scalingX
            : LSResizable.Method.Scale;
        var methodY = originalBounds.y < newSize.y
            ? resizable.scalingY
            : LSResizable.Method.Scale;
        var methodZ = originalBounds.z < newSize.z
            ? resizable.scalingZ
            : LSResizable.Method.Scale;

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
        LSResizable.Method resizeMethod,
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
            case LSResizable.Method.Adapt:
                if (Mathf.Abs(currentPosition) >= padding) {
                    currentPosition = resizedRatio * Mathf.Sign(currentPosition) + currentPosition;
                }

                break;

            case LSResizable.Method.AdaptWithAsymmetricalPadding:
                if (currentPosition >= padding) {
                    currentPosition = resizedRatio * Mathf.Sign(currentPosition) + currentPosition;
                }

                if (currentPosition <= paddingMax) {
                    currentPosition = resizedRatio * Mathf.Sign(currentPosition) + currentPosition;
                }

                break;

            case LSResizable.Method.Scale:
                currentPosition = newSize / (currentSize / currentPosition);
                break;

            case LSResizable.Method.None:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(resizeMethod), resizeMethod, null);
        }

        var pivotPos = newSize * -pivot;
        currentPosition += pivotPos;

        return currentPosition;
    }
}
