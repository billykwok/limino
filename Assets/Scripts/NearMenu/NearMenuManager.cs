using UnityEngine;

namespace NearMenu {
    public class NearMenuManager : MonoBehaviour {
        public bool IsHidden() {
            return !gameObject.activeSelf;
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        public void Summon(Vector3 menuPosition, Vector3 eyePosition) {
            transform.SetPositionAndRotation(menuPosition, Quaternion.LookRotation(menuPosition - eyePosition));
            gameObject.SetActive(true);
        }
    }
}
