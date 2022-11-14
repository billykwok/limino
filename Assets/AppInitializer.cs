using UnityEngine;

public class AppInitializer : MonoBehaviour {
    private void Start() {
        if (OVRManager.fixedFoveatedRenderingSupported) {
            OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.High;
            OVRManager.useDynamicFoveatedRendering = true;
        }
    }
}
