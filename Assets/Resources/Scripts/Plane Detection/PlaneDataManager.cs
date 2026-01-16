using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneDataManager
{
    // Store all PlaneData
    public List<PlaneData> planeDataContainer;

    public PlaneDataManager()
    {
        planeDataContainer = new List<PlaneData>();
    }

    public void GenerateFromARPlaneManager(ARPlaneManager arPlaneManager)
    {
        foreach (var trackablePlane in arPlaneManager.trackables)
        {
            PlaneData newPlaneData = PlaneDataGenerator.GenerateFromTrackable(trackablePlane);
            planeDataContainer.Add(newPlaneData);
        }
    }
}
