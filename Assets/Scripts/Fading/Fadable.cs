using UnityEngine;
using UnityEngine.EventSystems;

namespace Fading {
    [RequireComponent(typeof(BoxCollider), typeof(MeshRenderer))]
    public class Fadable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private static readonly int PROPERTY_OPACITY = Shader.PropertyToID("_InvertedAlpha");

        private MeshRenderer _meshRenderer;
        private bool _isUnlocked;
        private bool _isHovered;
        private float _opacity = 1.0f;
        private Material[] _materials;

        public void Lock() {
            _isUnlocked = false;
        }

        public void Unlock() {
            _isUnlocked = true;
        }

        private void Awake() {
            _meshRenderer = GetComponent<MeshRenderer>();
            _materials = _meshRenderer.materials;
        }

        private void LateUpdate() {
            if (_isUnlocked && _isHovered) {
                var thumbstickY = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).y;
                _opacity = Mathf.Clamp(_opacity + thumbstickY * 0.05f, 0, 1);
                OverrideOpacity(_opacity);
            }
        }

        public void OverrideOpacity(float opacity) {
            foreach (var material in _materials) {
                material.SetFloat(PROPERTY_OPACITY, opacity);
            }
        }

        public void OnPointerEnter(PointerEventData pointerEventData) {
            if (_isUnlocked) {
                _isHovered = true;
            }
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            if (_isUnlocked) {
                _isHovered = false;
            }
        }
    }
}
