using UnityEngine;

namespace PassthroughShadow {
    public class Flashlight : MonoBehaviour {
        public GameObject lightVolume;
        public Light spotlight;
        public GameObject bulbGlow;
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

        public void ToggleFlashlight() {
            lightVolume.SetActive(!lightVolume.activeSelf);
            spotlight.enabled = !spotlight.enabled;
            bulbGlow.SetActive(lightVolume.activeSelf);
        }

        public void DisableFlashlight() {
            lightVolume.SetActive(false);
            spotlight.enabled = false;
            bulbGlow.SetActive(false);
        }
    }
}
