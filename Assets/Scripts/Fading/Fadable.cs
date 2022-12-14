using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fading {
    [RequireComponent(typeof(BoxCollider), typeof(MeshRenderer))]
    public class Fadable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private static readonly int PROPERTY_COLOR = Shader.PropertyToID("_BaseColor");

        private MeshRenderer _meshRenderer;
        private bool _isUnlocked;
        private bool _isHovered;
        private float _opacity = 1.0f;
        private Material[] _materials;
        private Color[] _colors;

        public void Lock() {
            _isUnlocked = false;
        }

        public void Unlock() {
            _isUnlocked = true;
        }

        private void Awake() {
            _meshRenderer = GetComponent<MeshRenderer>();
            _materials = _meshRenderer.materials;
            _colors = _materials.Select(mat => mat.color).ToArray();
        }

        private void LateUpdate() {
            if (_isUnlocked && _isHovered) {
                var thumbstickY = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).y;
                _opacity = Mathf.Clamp(_opacity + thumbstickY * 0.05f, 0, 1);
                OverrideOpacity(_opacity);
            }
        }

        public void OverrideOpacity(float opacity) {
            for (var i = 0; i < _materials.Length; ++i) {
                _colors[i].a = opacity;
                _materials[i].SetColor(PROPERTY_COLOR, _colors[i]);
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
