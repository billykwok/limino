using System;
using GlobalFading;
using ObjectFading;
using PassthroughShapes;
using PassthroughStrokes;
using UnityEngine;

namespace EditMode {
    public class EditModeManager : MonoBehaviour {
        [SerializeField] private PassthroughStrokesManager _passthroughStrokesManager;
        [SerializeField] private PassthroughShapesManager _passthroughShapesManager;
        [SerializeField] private ObjectFadingManager _objectFadingManager;
        [SerializeField] private GlobalFadingManager _globalFadingManager;

        private void Start() {
            _passthroughStrokesManager.gameObject.SetActive(false);
            _passthroughShapesManager.gameObject.SetActive(false);
            _objectFadingManager.gameObject.SetActive(false);
            _globalFadingManager.gameObject.SetActive(false);
        }

        public void ActivatePassthroughStrokes() {
            _passthroughStrokesManager.gameObject.SetActive(true);
            _passthroughShapesManager.gameObject.SetActive(false);
            _objectFadingManager.gameObject.SetActive(false);
            _globalFadingManager.gameObject.SetActive(false);
        }

        public void ActivatePassthroughShapes() {
            _passthroughStrokesManager.gameObject.SetActive(false);
            _passthroughShapesManager.gameObject.SetActive(true);
            _objectFadingManager.gameObject.SetActive(false);
            _globalFadingManager.gameObject.SetActive(false);
        }

        public void ActivateObjectFading() {
            _passthroughStrokesManager.gameObject.SetActive(false);
            _passthroughShapesManager.gameObject.SetActive(false);
            _objectFadingManager.gameObject.SetActive(true);
            _globalFadingManager.gameObject.SetActive(false);
        }

        public void ActivateGlobalFading() {
            _passthroughStrokesManager.gameObject.SetActive(false);
            _passthroughShapesManager.gameObject.SetActive(false);
            _objectFadingManager.gameObject.SetActive(false);
            _globalFadingManager.gameObject.SetActive(true);
        }
    }
}
