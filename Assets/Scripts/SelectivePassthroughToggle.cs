using UnityEngine;

public class SelectivePassthroughToggle : MonoBehaviour {
    [SerializeField] private GameObject _leftControllerMask;
    [SerializeField] private GameObject _rightControllerMask;

    private void Update() {
        // when the button A of the right controller is pressed, 
        // the rightControllerMask gets enabled if its disabled and vice versa
        if (OVRInput.GetDown(OVRInput.Button.One)) {
            _rightControllerMask.SetActive(!_rightControllerMask.activeSelf);
        }

        // when the button X of the left controller is pressed, 
        // the leftControllerMask gets enabled if its disabled and vice versa
        if (OVRInput.GetDown(OVRInput.Button.Three)) {
            _leftControllerMask.SetActive(!_leftControllerMask.activeSelf);
        }
    }
}