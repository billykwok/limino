using UnityEngine;

public class LSPassthroughProjectionSurface : MonoBehaviour {
    public MeshFilter projectionObject;
    private OVRPassthroughLayer passthroughLayer;
    private MeshRenderer quadOutline;

    private void Start() {
        var ovrCameraRig = GameObject.Find("OVRCameraRig");
        if (ovrCameraRig == null) {
            Debug.LogError("Scene does not contain an OVRCameraRig");
            return;
        }

        passthroughLayer = ovrCameraRig.GetComponent<OVRPassthroughLayer>();
        if (passthroughLayer == null) {
            Debug.LogError("OVRCameraRig does not contain an OVRPassthroughLayer component");
        }

        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, true);

        // The MeshRenderer component renders the quad as a blue outline
        // we only use this when Passthrough isn't visible
        quadOutline = projectionObject.GetComponent<MeshRenderer>();
        quadOutline.enabled = false;
    }

    private void Update() {
        // Hide object when A button is held, show it again when button is released, move it while held.
        if (OVRInput.GetDown(OVRInput.Button.One)) {
            passthroughLayer.RemoveSurfaceGeometry(projectionObject.gameObject);
            quadOutline.enabled = true;
        }

        if (OVRInput.Get(OVRInput.Button.One)) {
            var controllingHand = OVRInput.Controller.RTouch;
            transform.position = OVRInput.GetLocalControllerPosition(controllingHand);
            transform.rotation = OVRInput.GetLocalControllerRotation(controllingHand);
        }

        if (OVRInput.GetUp(OVRInput.Button.One)) {
            passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject);
            quadOutline.enabled = false;
        }
    }
}
