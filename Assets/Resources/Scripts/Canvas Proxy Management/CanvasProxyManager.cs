using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CanvasProxyManager
{
    // Prefab to Clone for all Canvases
    GameObject canvasProxyPrefab;

    // Container for all Canvases
    public List<GameObject> canvasProxyContainer;

    public CanvasProxyManager()
    {
        canvasProxyContainer = new List<GameObject>();

        canvasProxyPrefab = Resources.Load<GameObject>("Prefabs/CanvasProxy");
    }

    public void GenerateProxiesFromPlaneData(PlaneDataManager planeDataManager)
    {
        foreach (var planeData in planeDataManager.planeDataContainer)
        {
            GameObject newCanvasProxy = GameObject.Instantiate(canvasProxyPrefab);

            // Anchor the Canvas
            newCanvasProxy.transform.SetParent(planeData.anchorParent.transform, false);

            // Initialize the Controller
            CanvasProxyController canvasProxyController = newCanvasProxy.GetComponent<CanvasProxyController>();
            canvasProxyController.Initalize(planeData);

            canvasProxyContainer.Add(newCanvasProxy);
        }
    }

    public void DuplicateFromProxyController(CanvasProxyController controller)
    {
        GameObject newCanvasProxy = GameObject.Instantiate(canvasProxyPrefab);

        // Keep Anchored at Original AR Anchor
        newCanvasProxy.transform.SetParent(controller.transform.parent, false);

        CanvasProxyController newController = newCanvasProxy.GetComponent<CanvasProxyController>();


        // Copy PlaneData to keep new Canvas Independent of the Other
        PlaneData originalPlaneData = controller.refPlaneData;
        PlaneData newPlaneData = new PlaneData
        {
            centerPosition = originalPlaneData.centerPosition,
            normal = originalPlaneData.normal,
            localBoundaryPoints = new List<Vector3>(originalPlaneData.localBoundaryPoints),
            halfSize = originalPlaneData.halfSize,
            anchorParent = originalPlaneData.anchorParent
        };

        newController.Initalize(newPlaneData);

        canvasProxyContainer.Add(newCanvasProxy);
    }
}
