using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class PlaneDataGenerator
{
    public static PlaneData GenerateFromTrackable(ARPlane arPlane)
    {
        PlaneData planeData = new PlaneData();

        // Keep a Reference to the ARPlane
        planeData.anchorParent = arPlane;
        
        planeData.centerPosition = arPlane.center;
        planeData.normal = arPlane.normal;

        // Boundary Vertices must be converted to World Space
        planeData.localBoundaryPoints = new List<Vector3>();

        // Extrapolate to a Rectangular Shape
        float halfWidth = arPlane.extents.x;
        float halfHeight = arPlane.extents.y;

        Vector3 v0 = new Vector3(-halfWidth, 0, -halfHeight);
        Vector3 v1 = new Vector3(+halfWidth, 0, -halfHeight);
        Vector3 v2 = new Vector3(+halfWidth, 0, +halfHeight);
        Vector3 v3 = new Vector3(-halfWidth, 0, +halfHeight);

        planeData.localBoundaryPoints.Add(v0);
        planeData.localBoundaryPoints.Add(v1);
        planeData.localBoundaryPoints.Add(v2);
        planeData.localBoundaryPoints.Add(v3);

        planeData.halfSize = new Vector2(halfWidth, halfHeight);

        return planeData;
    }
}
