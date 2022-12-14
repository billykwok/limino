using System.Collections;
using UnityEngine;
using TMPro;

public class HeadsetMovementDetection : MonoBehaviour {
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Material material1;
    [SerializeField] private Material material2;
    [SerializeField] private GameObject headsetObj;

    public TextMeshProUGUI debuggertext;
    public TextMeshProUGUI debuggertext2;

    private Vector3 lastPos;
    private float threshold = 0.2f;

    public void OnEnable() => StartCoroutine(DetectionTimer());

    IEnumerator DetectionTimer(float countTime = 1f) {
        int count = 0;
        while (true) {
            yield return new WaitForSeconds(countTime);
            count++;

            Vector3 offset = headsetObj.transform.position - lastPos;
            if (offset.y != 0f) {
                if (offset.y > threshold) {
                    targetObject.GetComponent<MeshRenderer>().material = material1;
                } else if (offset.y < -threshold) {
                    targetObject.GetComponent<MeshRenderer>().material = material2;
                }

                lastPos = headsetObj.transform.position;
            }

            debuggertext.text = lastPos.ToString();
            debuggertext2.text = offset.ToString();
        }
    }
}
