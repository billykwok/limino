using UnityEngine;

[RequireComponent(typeof(OVRManager))]
public class LSPassthroughPlayInEditor : MonoBehaviour {
    private void Awake() {
#if UNITY_EDITOR
        GetComponent<OVRManager>().isInsightPassthroughEnabled = false;
        var passthroughLayer = GetComponent<OVRPassthroughLayer>();
        if (passthroughLayer) {
            passthroughLayer.enabled = false;
        }
#endif
    }
}
