using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneData
{
    public Vector3 centerPosition; // World Position
    public Vector3 normal;
    public List<Vector3> localBoundaryPoints; // 4 Vertices in Local Space
    public Vector2 halfSize; // Half Width, Half Height

    // AR Anchoring
    public ARPlane anchorParent;
}
