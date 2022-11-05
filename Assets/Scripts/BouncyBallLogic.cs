using System.Collections;
using UnityEngine;

public class BouncyBallLogic : MonoBehaviour {
    [SerializeField] private float ttl = 5.0f;
    [SerializeField] private AudioClip audioPop;
    [SerializeField] private AudioClip audioBounce;
    [SerializeField] private AudioClip audioLoading;
    [SerializeField] private Material visibleMat;
    [SerializeField] private Material hiddenMat;

    private AudioSource _audioSource;
    private Transform _centerEyeCamera;
    private bool _isReadyForDestroy;
    private bool _isReleased;
    private bool _isVisible = true;
    private float _timer;

    private void Start() {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.PlayOneShot(audioLoading);
        _centerEyeCamera = OVRManager.instance.GetComponentInChildren<OVRCameraRig>().centerEyeAnchor;
    }


    private void Update() {
        if (!_isReleased) {
            return;
        }

        UpdateVisibility();
        _timer += Time.deltaTime;
        if (!_isReadyForDestroy && _timer >= ttl) {
            _isReadyForDestroy = true;
            var clipLength = audioPop.length;
            _audioSource.PlayOneShot(audioPop);
            StartCoroutine(PlayPopCallback(clipLength));
        }
    }

    private void OnCollisionEnter() {
        _audioSource.PlayOneShot(audioBounce);
    }

    private void UpdateVisibility() {
        var position = transform.position;
        var displacement = _centerEyeCamera.position - position;
        var ray = new Ray(position, displacement);
        RaycastHit info;
        if (Physics.Raycast(ray, out info, displacement.magnitude)) {
            if (info.collider.gameObject != gameObject) {
                SetVisible(false);
            }
        }
        else {
            SetVisible(true);
        }
    }

    private void SetVisible(bool setVisible) {
        if (_isVisible && !setVisible) {
            GetComponent<MeshRenderer>().material = hiddenMat;
            _isVisible = false;
        }

        if (!_isVisible && setVisible) {
            GetComponent<MeshRenderer>().material = visibleMat;
            _isVisible = true;
        }
    }

    public void Release(Vector3 pos, Vector3 vel, Vector3 angVel) {
        _isReleased = true;
        transform.position = pos; // set the origin to match target
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = vel;
        GetComponent<Rigidbody>().angularVelocity = angVel;
    }

    private IEnumerator PlayPopCallback(float clipLength) {
        yield return new WaitForSeconds(clipLength);
        Destroy(gameObject);
    }
}