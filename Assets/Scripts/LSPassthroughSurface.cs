using UnityEngine;

public class LSPassthroughSurface : MonoBehaviour {
    public OVRPassthroughLayer passthroughLayer;
    public MeshFilter projectionObject;

    private void Start() {
        Destroy(projectionObject.GetComponent<MeshRenderer>());
        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, true);
    }
}
