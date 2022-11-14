using UnityEngine;

public class LSRequestCaptureFlow : MonoBehaviour {
    public OVRInput.Button requestCaptureBtn = OVRInput.Button.Two;

    private OVRSceneManager _sceneManager;

    private void Start() {
        _sceneManager = FindObjectOfType<OVRSceneManager>();
    }

    // Update is called once per frame
    private void Update() {
        if (OVRInput.GetUp(requestCaptureBtn)) {
            _sceneManager.RequestSceneCapture();
        }
    }
}
