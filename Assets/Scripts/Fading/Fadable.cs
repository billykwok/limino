using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fading {
    [RequireComponent(typeof(BoxCollider), typeof(MeshRenderer))]
    public class Fadable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private static readonly int PROPERTY_OPACITY = Shader.PropertyToID("_InvertedAlpha");
        private static readonly int PROPERTY_COLOR = Shader.PropertyToID("_BaseColor");

        private MeshRenderer _meshRenderer;
        private string _classification = OVRSceneManager.Classification.Other;
        private bool _isUnlocked;
        private bool _isHovered;
        private float _passthroughOpacity;
        private Material[] _materials;
        private Color[] _colors;

        public void SetClassification(string classification) {
            _classification = classification;
        }

        public string GetClassification() {
            return _classification;
        }

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
                _passthroughOpacity = Mathf.Clamp(_passthroughOpacity + thumbstickY * 0.05f, 0, 1);
                OverrideOpacity(_passthroughOpacity);
            }
        }

        public void ResetOpacity() {
            OverrideOpacity(_passthroughOpacity);
        }

        public void OverrideOpacity(float opacity) {
            for (var i = 0; i < _materials.Length; ++i) {
                var material = _materials[i];
                if (_classification is OVRSceneManager.Classification.WallFace
                    or OVRSceneManager.Classification.Ceiling) {
                    material.SetFloat(PROPERTY_OPACITY, opacity);
                } else {
                    var color = _colors[i];
                    material.SetColor(
                        PROPERTY_COLOR,
                        new Color(color.r, color.g, color.b, Mathf.Lerp(0, color.a, 1 - opacity))
                    );
                    material.SetFloat(PROPERTY_OPACITY, opacity);
                }
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
