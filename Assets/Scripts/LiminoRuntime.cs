using UnityEngine;

public class LiminoRuntime : MonoBehaviour {
    private void Start() {
        OVRManager.suggestedCpuPerfLevel = OVRManager.ProcessorPerformanceLevel.Boost;
        OVRManager.suggestedGpuPerfLevel = OVRManager.ProcessorPerformanceLevel.Boost;
        OVRManager.eyeFovPremultipliedAlphaModeEnabled = false;
        if (OVRManager.fixedFoveatedRenderingSupported || OVRManager.eyeTrackedFoveatedRenderingSupported) {
            OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.HighTop;
            OVRManager.useDynamicFoveatedRendering = true;
        }
    }
}
