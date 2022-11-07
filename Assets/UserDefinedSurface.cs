using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDefinedSurface : MonoBehaviour
{
    [SerializeField] private OVRPassthroughLayer passthroughLayer;
    [SerializeField] private bool updateTransform = false;

    private void Start()
    {
        this.passthroughLayer.AddSurfaceGeometry(this.gameObject, this.updateTransform);
    }
}
