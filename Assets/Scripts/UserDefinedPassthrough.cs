using UnityEngine;

public class UserDefinedPassthrough : MonoBehaviour {
    [SerializeField] private OVRPassthroughLayer _passthroughLayer;
    [SerializeField] private bool _updateTransform;

    private void Start() {
        _passthroughLayer.AddSurfaceGeometry(gameObject, _updateTransform);
    }
}