using UnityEngine;
using UnityEngine.Events;

namespace InputSystem {
    public class Raycastable : MonoBehaviour {
        [SerializeField] private UnityEvent rayEnterEvent = new();
        [SerializeField] private UnityEvent rayExitEvent = new();

        public void OnRayEnter() {
            rayEnterEvent.Invoke();
        }

        public void OnRayExit() {
            rayExitEvent.Invoke();
        }

        public void AddRayEnterListener(UnityAction action) {
            rayEnterEvent.AddListener(action);
        }

        public void AddRayExitListener(UnityAction action) {
            rayExitEvent.AddListener(action);
        }

        public void RemoveRayEnterListener(UnityAction action) {
            rayEnterEvent.RemoveListener(action);
        }

        public void RemoveRayExitListener(UnityAction action) {
            rayExitEvent.RemoveListener(action);
        }
    }
}
