using UnityEngine;

[ExecuteInEditMode]
public class Resizable : MonoBehaviour {
    public enum Method {
        Adapt,
        AdaptWithAsymmetricalPadding,
        Scale,
        None
    }

    [Space(15)] public Method scalingX;
    [Space(15)] public Method scalingY;
    [Space(15)] public Method scalingZ;

    [Range(0, 0.5f)] public float paddingX;
    [Range(0, 0.5f)] public float paddingY;
    [Range(0, 0.5f)] public float paddingZ;

    [Range(-0.5f, 0)] public float paddingXMax;
    [Range(-0.5f, 0)] public float paddingYMax;
    [Range(-0.5f, 0)] public float paddingZMax;

    [SerializeField] private Transform _pivotTransform;

    private Bounds _bounds;
    public Vector3 pivotPosition => _pivotTransform.position; //Vector3.zero;

    public Vector3 newSize { get; set; }
    public Vector3 defaultSize { get; private set; }
    public Mesh mesh { get; private set; }

    private void Awake() {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        defaultSize = mesh.bounds.size;
        if (!_pivotTransform) {
            _pivotTransform = transform.Find("Pivot");
        }
    }

#if UNITY_EDITOR
    private void OnEnable() {
        defaultSize = mesh.bounds.size;
        newSize = defaultSize;
    }

    private void OnDrawGizmos() {
        if (!_pivotTransform) {
            return;
        }

        Gizmos.color = Color.red;
        var lineSize = 0.1f;

        var startX = _pivotTransform.position + Vector3.left * lineSize * 0.5f;
        var startY = _pivotTransform.position + Vector3.down * lineSize * 0.5f;
        var startZ = _pivotTransform.position + Vector3.back * lineSize * 0.5f;

        Gizmos.DrawRay(startX, Vector3.right * lineSize);
        Gizmos.DrawRay(startY, Vector3.up * lineSize);
        Gizmos.DrawRay(startZ, Vector3.forward * lineSize);
    }

    private void OnDrawGizmosSelected() {
        defaultSize = mesh.bounds.size;

        if (GetComponent<MeshFilter>().sharedMesh == null) {
            // The furniture piece was not customized yet, nothing to do here
            return;
        }

        _bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
        Gizmos.matrix = transform.localToWorldMatrix;
        var newCenter = _bounds.center;

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        switch (scalingX) {
            case Method.Adapt:
                Gizmos.DrawWireCube(newCenter, new Vector3(newSize.x * paddingX * 2, newSize.y, newSize.z));
                break;

            case Method.AdaptWithAsymmetricalPadding:
                Gizmos.DrawWireCube(
                    newCenter + new Vector3(newSize.x * paddingX, 0, 0),
                    new Vector3(0, newSize.y, newSize.z)
                );
                Gizmos.DrawWireCube(
                    newCenter + new Vector3(newSize.x * paddingXMax, 0, 0),
                    new Vector3(0, newSize.y, newSize.z)
                );
                break;

            case Method.None:
                Gizmos.DrawWireCube(newCenter, newSize);
                break;
        }

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        switch (scalingY) {
            case Method.Adapt:
                Gizmos.DrawWireCube(newCenter, new Vector3(newSize.x, newSize.y * paddingY * 2, newSize.z));
                break;

            case Method.AdaptWithAsymmetricalPadding:
                Gizmos.DrawWireCube(
                    newCenter + new Vector3(0, newSize.y * paddingY, 0),
                    new Vector3(newSize.x, 0, newSize.z)
                );
                Gizmos.DrawWireCube(
                    newCenter + new Vector3(0, newSize.y * paddingYMax, 0),
                    new Vector3(newSize.x, 0, newSize.z)
                );
                break;

            case Method.None:
                Gizmos.DrawWireCube(newCenter, newSize);
                break;
        }

        Gizmos.color = new Color(0, 0, 1, 0.5f);
        switch (scalingZ) {
            case Method.Adapt:
                Gizmos.DrawWireCube(newCenter, new Vector3(newSize.x, newSize.y, newSize.z * paddingZ * 2));
                break;

            case Method.AdaptWithAsymmetricalPadding:
                Gizmos.DrawWireCube(
                    newCenter + new Vector3(0, 0, newSize.z * paddingZ),
                    new Vector3(newSize.x, newSize.y, 0)
                );
                Gizmos.DrawWireCube(
                    newCenter + new Vector3(0, 0, newSize.z * paddingZMax),
                    new Vector3(newSize.x, newSize.y, 0)
                );
                break;

            case Method.None:
                Gizmos.DrawWireCube(newCenter, newSize);
                break;
        }

        Gizmos.color = new Color(0, 1, 1, 1);
        Gizmos.DrawWireCube(newCenter, newSize);
    }
#endif
}