using System.Linq;
using InputSystem;
using UnityEngine;

namespace ObjectFading {
    [RequireComponent(typeof(BoxCollider), typeof(Raycastable), typeof(MeshRenderer))]
    public class Fadable : MonoBehaviour {
        private static readonly int PROPERTY_COLOR = Shader.PropertyToID("_BaseColor");

        private Raycastable _raycastable;
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
            _raycastable = GetComponent<Raycastable>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _materials = _meshRenderer.materials;
            _colors = _materials.Select(mat => mat.color).ToArray();
        }

        private void OnEnable() {
            _raycastable.AddRayEnterListener(OnRayEnter);
            _raycastable.AddRayExitListener(OnRayExit);
        }

        private void Update() {
            if (_isUnlocked && _isHovered) {
                var thumbstickY = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).y;
                _opacity = Mathf.Clamp(_opacity + thumbstickY * 0.05f, 0, 1);
                for (var i = 0; i < _materials.Length; ++i) {
                    _colors[i].a = _opacity;
                    _materials[i].SetColor(PROPERTY_COLOR, _colors[i]);
                }
            }
        }

        private void OnDisable() {
            _raycastable.RemoveRayEnterListener(OnRayEnter);
            _raycastable.RemoveRayExitListener(OnRayExit);
        }

        private void OnRayEnter() {
            if (_isUnlocked) {
                _isHovered = true;
            }
        }

        private void OnRayExit() {
            if (_isUnlocked) {
                _isHovered = false;
            }
        }
    }
}
