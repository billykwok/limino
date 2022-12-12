using UnityEngine;

namespace Casting {
    public class Torch : MonoBehaviour {
        public GameObject lightVolume;
        private Camera _camera;

        private void Awake() {
            _camera = Camera.main;
        }

        private void LateUpdate() {
            for (var i = 0; i < lightVolume.transform.childCount; i++) {
                lightVolume.transform.GetChild(i).rotation = Quaternion.LookRotation(
                    (lightVolume.transform.GetChild(i).position - _camera.transform.position).normalized
                );
            }
        }
    }
}
