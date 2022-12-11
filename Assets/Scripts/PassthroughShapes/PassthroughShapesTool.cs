using UnityEngine;
using Utils;

namespace PassthroughShapes {
    public class PassthroughShapesTool : MonoBehaviour, IBlendingTool {
        private void Start() {
            gameObject.SetActive(false);
        }

        public void onActivate() {
            throw new System.NotImplementedException();
        }

        public void onDeactivate() {
            throw new System.NotImplementedException();
        }
    }
}
