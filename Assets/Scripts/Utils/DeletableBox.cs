using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils {
    [RequireComponent(typeof(MeshFilter), typeof(BoxCollider))]
    public class DeletableBox : MonoBehaviour, IPointerMoveHandler {
        private MeshFilter _meshFilter;
        private BoxCollider _boxCollider;

        private void Awake() {
            _meshFilter = GetComponent<MeshFilter>();
            _boxCollider = GetComponent<BoxCollider>();
            var mesh = _meshFilter.mesh;
            _boxCollider.size = mesh.bounds.size;
            _boxCollider.center = mesh.bounds.center;
        }

        public void OnPointerMove(PointerEventData eventData) {
            if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
                Destroy(gameObject);
            }
        }
    }
}
