using UnityEngine;
using Utils;

namespace ObjectFading {
    public class ObjectFadingTool : MonoBehaviour, IBlendingTool {
        private Fadable[] _fadables;

        public void onActivate() {
            _fadables = FindObjectsOfType<Fadable>(true);
            foreach (var fadable in _fadables) {
                fadable.Unlock();
            }
        }

        public void onDeactivate() {
            _fadables = FindObjectsOfType<Fadable>(true);
            foreach (var fadable in _fadables) {
                fadable.Lock();
            }
        }
    }
}
