using UnityEngine;
using Utils;

namespace Fading {
    public class FadingTool : MonoBehaviour, IBlendingTool {
        public enum Mode {
            Object,
            Global
        }

        private Fadable[] _fadables;
        private Mode _mode = Mode.Object;
        private float _globalOpacity = 0.5f;

        public void OnSelect() {
            if (_mode == Mode.Object) {
                _fadables = FindObjectsOfType<Fadable>(true);
                foreach (var fadable in _fadables) {
                    fadable.Unlock();
                }
            }
        }

        public void OnDeselect() {
            _fadables = FindObjectsOfType<Fadable>(true);
            foreach (var fadable in _fadables) {
                fadable.Lock();
            }
        }

        public void Lock() {
        }

        public void Unlock() {
        }

        public void SwitchMode(Mode mode) {
            if (mode == Mode.Global && _mode == Mode.Object) {
                OverrideGlobalOpacity(_globalOpacity);
            } else if (mode == Mode.Object && _mode == Mode.Global) {
                ResetObjectOpacity();
            }

            _mode = mode;
        }

        public void OverrideGlobalOpacity(float globalOpacity) {
            _fadables = FindObjectsOfType<Fadable>(true);
            foreach (var fadable in _fadables) {
                fadable.Lock();
                fadable.OverrideOpacity(globalOpacity);
            }
        }

        private void ResetObjectOpacity() {
            _fadables = FindObjectsOfType<Fadable>(true);
            foreach (var fadable in _fadables) {
                fadable.ResetOpacity();
                fadable.Unlock();
            }
        }

        public void OverrideDoorOpacity(float doorOpacity) {
            _fadables = FindObjectsOfType<Fadable>(true);
            foreach (var fadable in _fadables) {
                if (fadable.GetClassification() == OVRSceneManager.Classification.DoorFrame) {
                    fadable.OverrideOpacity(doorOpacity);
                }
            }
        }

        private void LateUpdate() {
            if (_mode == Mode.Global) {
                var thumbstickY = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).y;
                _globalOpacity += thumbstickY * 0.05f;
                OverrideGlobalOpacity(_globalOpacity);
            }
        }
    }
}
