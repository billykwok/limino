using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils {
    [RequireComponent(typeof(LineRenderer), typeof(MeshCollider))]
    public class DeletableLine : MonoBehaviour, IPointerMoveHandler {
        private LineRenderer _lineRenderer;
        private MeshCollider _meshCollider;
        private Camera _camera;

        private void Awake() {
            _camera = Camera.main;
            _lineRenderer = GetComponent<LineRenderer>();
            _meshCollider = GetComponent<MeshCollider>();
            var mesh = new Mesh();
            _lineRenderer.BakeMesh(mesh, _camera, true);
            _meshCollider.sharedMesh = mesh;
        }

        public void OnPointerMove(PointerEventData eventData) {
            if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
                Destroy(gameObject);
            }
        }
    }
}
