using UnityEngine;

public class CanvasProxyController : MonoBehaviour
{
    // Component References
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    // Render Texture for Painting
    public RenderTexture renderTexture;

    // Reference to PlaneData
    public PlaneData refPlaneData;

    public void Initalize(PlaneData planeData)
    {
        //
        // Component References must be set by Inspected already
        //

        // Create Render Texture
        renderTexture = new RenderTexture(1024, 1024, 0);
        renderTexture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        // Fill with White Color
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.white);

        // Assign RenderTexture to our Material
        meshRenderer.material.mainTexture = renderTexture;

        // Generate Mesh
        Mesh mesh = new Mesh();

        // Generate Mesh Parameters
        Vector3[] vertices = planeData.localBoundaryPoints.ToArray(); // Must be exactly 4 Vertices
        int[] triangles = { 0, 2, 1, 0, 3, 2 }; // Rectangle Triangulated, Forward Facing
        Vector2[] uvs = {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,1)
        };

        // Build the Mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Create a New Mesh for the Filter and Assign our generated mesh
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        refPlaneData = planeData;
    }

    public void Resize(float width, float height)
    {
        // Update half sizes in PlaneData
        refPlaneData.halfSize = new Vector2(width * 0.5f, height * 0.5f);

        float halfWidth = refPlaneData.halfSize.x;
        float halfHeight = refPlaneData.halfSize.y;

        // Recalculate local boundary points (rectangle)
        refPlaneData.localBoundaryPoints[0] = new Vector3(-halfWidth, 0, -halfHeight);
        refPlaneData.localBoundaryPoints[1] = new Vector3(+halfWidth, 0, -halfHeight);
        refPlaneData.localBoundaryPoints[2] = new Vector3(+halfWidth, 0, +halfHeight);
        refPlaneData.localBoundaryPoints[3] = new Vector3(-halfWidth, 0, +halfHeight);

        // Update Mesh
        Mesh mesh = meshFilter.mesh;
        mesh.vertices = refPlaneData.localBoundaryPoints.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        // Resize RenderTexture to match resolution
        if (renderTexture != null)
        {
            int newWidth = Mathf.RoundToInt(width * 1024f);  // Scale factor example
            int newHeight = Mathf.RoundToInt(height * 1024f);

            RenderTexture newRT = new RenderTexture(newWidth, newHeight, 0);
            newRT.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;
            newRT.enableRandomWrite = true;
            newRT.Create();

            // Copy old texture content
            Graphics.Blit(renderTexture, newRT);

            renderTexture.Release();
            renderTexture = newRT;
            meshRenderer.material.mainTexture = renderTexture;
        }
    }
}
