using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using UnityEngine.InputSystem;

// Manages Google ARCore Systems
// * Responsible for Toggling On/Off Plane Detection and Occlusion (Optional, may not even use this).
// * Responsible for Performing Raycasts on Planes
public class ARSystemsController : MonoBehaviour
{
    // Events
    public event Action OnPlaneDetectionComplete;
    
    // References to AR System that the App uses
    public ARPlaneManager refPlaneManager;
    public Camera refCamera;
    public AROcclusionManager refOcclusionManager;

    bool isPlaneDetectionComplete = false;

    void Start()
    {
        // Grab References
        refPlaneManager = GetComponent<ARPlaneManager>();
        refCamera = GetComponentInChildren<Camera>(); // Main Camera
        refOcclusionManager = GetComponentInChildren<AROcclusionManager>(); // Occlusion Manager exists in the Main Camera
    }

    public void TogglePlaneDetection()
    {
        refPlaneManager.enabled = false;
        // Notify Subscribers only Once
        if (!isPlaneDetectionComplete)
        {
            OnPlaneDetectionComplete?.Invoke();

            // Hide AR Visuals
            foreach (var plane in refPlaneManager.trackables) 
            {
                var visualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
                if (visualizer != null)
                {
                    visualizer.enabled = false;
                }
            }

            isPlaneDetectionComplete = true;
        }

    }

    public void ToggleOcclusion()
    {
        refOcclusionManager.enabled = !refOcclusionManager.enabled;
    }
}
