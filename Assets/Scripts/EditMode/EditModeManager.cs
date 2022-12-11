using System;
using GlobalFading;
using ObjectFading;
using PassthroughShapes;
using PassthroughStrokes;
using UnityEngine;
using UnityEngine.Serialization;

namespace EditMode {
    public class EditModeManager : MonoBehaviour {
        [FormerlySerializedAs("_passthroughStrokesManager")] [SerializeField]
        private PassthroughStrokesTool passthroughStrokesTool;

        [FormerlySerializedAs("_passthroughShapesManager")] [SerializeField]
        private PassthroughShapesTool passthroughShapesTool;

        [FormerlySerializedAs("_objectFadingManager")] [SerializeField]
        private ObjectFadingTool objectFadingTool;

        [FormerlySerializedAs("_globalFadingManager")] [SerializeField]
        private GlobalFadingTool globalFadingTool;

        private void Start() {
            passthroughStrokesTool.gameObject.SetActive(false);
            passthroughShapesTool.gameObject.SetActive(false);
            objectFadingTool.onDeactivate();
            globalFadingTool.gameObject.SetActive(false);
        }

        public void ActivatePassthroughStrokes() {
            passthroughStrokesTool.gameObject.SetActive(true);
            passthroughShapesTool.gameObject.SetActive(false);
            objectFadingTool.onDeactivate();
            globalFadingTool.gameObject.SetActive(false);
        }

        public void ActivatePassthroughShapes() {
            passthroughStrokesTool.gameObject.SetActive(false);
            passthroughShapesTool.gameObject.SetActive(true);
            objectFadingTool.onDeactivate();
            globalFadingTool.gameObject.SetActive(false);
        }

        public void ActivateObjectFading() {
            passthroughStrokesTool.gameObject.SetActive(false);
            passthroughShapesTool.gameObject.SetActive(false);
            objectFadingTool.onActivate();
            globalFadingTool.gameObject.SetActive(false);
        }

        public void ActivateGlobalFading() {
            passthroughStrokesTool.gameObject.SetActive(false);
            passthroughShapesTool.gameObject.SetActive(false);
            objectFadingTool.onDeactivate();
            globalFadingTool.gameObject.SetActive(true);
        }
    }
}
