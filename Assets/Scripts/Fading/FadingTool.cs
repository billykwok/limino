using UnityEngine;
using Utils;

namespace Fading {
    public class FadingTool : MonoBehaviour, IBlendingTool {
        private Fadable[] _fadables;

        public void OnSelect() {
            _fadables = FindObjectsOfType<Fadable>(true);
            foreach (var fadable in _fadables) {
                fadable.Unlock();
            }
        }

        public void OnDeselect() {
            _fadables = FindObjectsOfType<Fadable>(true);
            foreach (var fadable in _fadables) {
                fadable.Lock();
            }
        }
    }
}
