using UnityEngine;

public class LSBouncingBallManager : MonoBehaviour {
    [SerializeField] private Transform trackingSpace;
    [SerializeField] private GameObject rightControllerPivot;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private bool _ballGrabbed;
    private GameObject _currentBall;

    private void Update() {
        if (!_ballGrabbed && OVRInput.GetDown(actionBtn)) {
            _currentBall = Instantiate(ball, rightControllerPivot.transform.position, Quaternion.identity);
            _currentBall.transform.parent = rightControllerPivot.transform;
            _ballGrabbed = true;
        }

        if (_ballGrabbed && OVRInput.GetUp(actionBtn)) {
            _currentBall.transform.parent = null;
            var ballPos = _currentBall.transform.position;
            var vel = trackingSpace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
            _currentBall.GetComponent<LSBouncingBallLogic>().Release(ballPos, vel, angVel);
            _ballGrabbed = false;
        }
    }
}
