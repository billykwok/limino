using System;
using System.Collections.Generic;
using UnityEngine;

public class SpatialAnchorsManager : MonoBehaviour {
    private const ulong INVALID_HANDLE = ulong.MaxValue;
    private readonly Vector3 _objectOffset = new(0, 0, 0.1f);
    private Dictionary<ulong, AnchorData> _createdAnchors;
    private GameObject _heldCube;
    private GameObject _heldSphere;
    private Camera _mainCamera;

    private void Start() {
        OVRManager.suggestedCpuPerfLevel = OVRManager.ProcessorPerformanceLevel.SustainedHigh;
        OVRManager.suggestedGpuPerfLevel = OVRManager.ProcessorPerformanceLevel.SustainedHigh;
        _mainCamera = Camera.main;
        _heldCube = GameObject.Find("HeldCube");
        _heldSphere = GameObject.Find("HeldSphere");
        _createdAnchors = new Dictionary<ulong, AnchorData>();
    }

    private void Update() {
        var trigger1Pressed = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
        var trigger2Pressed = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);

        if (trigger1Pressed) {
            GenerateSpatialAnchorForController(true);
        }

        if (trigger2Pressed) {
            GenerateSpatialAnchorForController(false);
        }
    }

    private void OnEnable() {
        OVRManager.SpaceSetComponentStatusComplete += OVRManager_SpaceSetComponentStatusComplete;
    }

    private void OnDisable() {
        OVRManager.SpaceSetComponentStatusComplete -= OVRManager_SpaceSetComponentStatusComplete;
    }

    private void GenerateObject(bool isLeft) {
        //OVRPose objectPose = new OVRPose()
        //{
        //    position = OVRInput.GetLocalControllerPosition(isLeft ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch) + objectOffset,
        //    orientation = OVRInput.GetLocalControllerRotation(isLeft ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch)
        //};
        //OVRPose worldObjectPose = OVRExtensions.ToWorldSpacePose(objectPose, mainCamera);

        Instantiate(
            Resources.Load<GameObject>(isLeft ? "Cube" : "Sphere"),
            (isLeft ? _heldCube : _heldSphere).transform.position,
            (isLeft ? _heldCube : _heldSphere).transform.rotation
        );
    }

    private void GenerateSpatialAnchorForController(bool isLeft) {
        var controller = isLeft
            ? OVRInput.Controller.LTouch
            : OVRInput.Controller.RTouch;
        var controllerPose = new OVRPose {
            position = OVRInput.GetLocalControllerPosition(controller),
            orientation = OVRInput.GetLocalControllerRotation(controller)
        };

        var createInfo = new OVRPlugin.SpatialAnchorCreateInfo {
            Time = OVRPlugin.GetTimeInSeconds(),
            BaseTracking = OVRPlugin.GetTrackingOriginType(),
            PoseInSpace = controllerPose.ToPosef()
        };

        ulong requestId = 0;
        if (OVRPlugin.CreateSpatialAnchor(createInfo, out requestId)) {
            var spaceHandle = INVALID_HANDLE;
            Debug.Log($"spaceHandle: {spaceHandle}");
            _createdAnchors[spaceHandle] = new AnchorData {
                spaceHandle = spaceHandle,
                prefabName = isLeft ? "Cube" : "Sphere"
            };
            if (OVRPlugin.GetSpaceComponentStatus(
                    spaceHandle,
                    OVRPlugin.SpaceComponentType.Locatable,
                    out var componentEnabled,
                    out var changePending
                )) {
                if (!componentEnabled) {
                    if (!OVRPlugin.SetSpaceComponentStatus(
                            spaceHandle,
                            OVRPlugin.SpaceComponentType.Locatable,
                            true,
                            0,
                            out requestId
                        )) {
                        Debug.LogError("Addition of Locatable component to spatial anchor failed");
                    }
                } else {
                    GenerateOrUpdateGameObjectForAnchor(spaceHandle);
                }
            } else {
                Debug.LogError("Get status of Locatable component to spatial anchor failed");
            }
        } else {
            Debug.LogError("Creation of spatial anchor failed");
        }
    }

    private void GenerateOrUpdateGameObjectForAnchor(ulong spaceHandle) {
        if (_createdAnchors[spaceHandle].instantiatedObject == null) {
            _createdAnchors[spaceHandle].instantiatedObject =
                Instantiate(Resources.Load<GameObject>(_createdAnchors[spaceHandle].prefabName));
        }

        var anchorPose = OVRPlugin.LocateSpace(spaceHandle, OVRPlugin.GetTrackingOriginType());
        var anchorPoseUnity = anchorPose.ToOVRPose().ToWorldSpacePose(_mainCamera);

        _createdAnchors[spaceHandle].instantiatedObject.transform
            .SetPositionAndRotation(anchorPoseUnity.position, anchorPoseUnity.orientation);
        Debug.Log($"Placed at {anchorPoseUnity.position}");
    }

    private void OVRManager_SpaceSetComponentStatusComplete(
        ulong requestId,
        bool result,
        OVRSpace space,
        Guid uuid,
        OVRPlugin.SpaceComponentType componentType,
        bool isEnabled
    ) {
        if (result) {
            if (!_createdAnchors.ContainsKey(space.Handle)) {
                Debug.LogError("Asked to activate a component on an unknown anchor, aborting");
                return;
            }

            GenerateOrUpdateGameObjectForAnchor(space.Handle);
            Debug.Log($"Addition of {componentType} component to spatial anchor successfully completed");
        } else {
            Debug.LogError($"Addition of {componentType} component to spatial anchor failed");
        }
    }

    [Serializable]
    public class AnchorData {
        public ulong spaceHandle;
        public string prefabName;
        public GameObject instantiatedObject;
    }
}