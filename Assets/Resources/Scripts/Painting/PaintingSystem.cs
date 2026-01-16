using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PaintingSystem
{
    ComputeShader batchedPaintShader;

    public int numInterpolatedPixels = 16;
    public int maxPaintBatchSize = 32; // Maximum Paint Jobs Per Update
    public int maxShaderPixelCount = 64;

    const int THREAD_GROUP_SIZE = 8;

    int paintShaderKernel;

    struct PaintColorTask
    {
        public Vector2Int pixelCoords;
    };

    // Store Tasks in a Buffer before bieng pushed to Queue
    List<PaintColorTask> paintColorTaskList;

    List<PaintColorTask> paintTasksQueue;


    // We will ignore any new Canvases until Pointer is lifted
    CanvasProxyController lastCanvasProxyController; 
    public int useBrushSize;
    public Color useColor;
    Vector2Int lastProperPixelCoords; // Stores last pointer associated pixel (not interpolated pixels)

    bool shouldQueueAllTasks = false;

    public PaintingSystem()
    {
        batchedPaintShader = Resources.Load<ComputeShader>("Scripts/Painting/BatchedPaintShader");
        
        // Store it at Start for Faster Processing
        paintShaderKernel = batchedPaintShader.FindKernel("CSMain");

        paintColorTaskList = new List<PaintColorTask>();
        paintTasksQueue = new List<PaintColorTask>();
        shouldQueueAllTasks = false;
    }

    public void PaintTexture(CanvasProxyController canvasProxy, GameObject imageGameObject)
    {
        // Get the Image
        Image selectedImage = imageGameObject.GetComponent<Image>();

        // Remove GIF Animation if it exists
        GIFManager.Instance.RemoveGIF(canvasProxy);

        Sprite spriteImage = selectedImage.sprite;

        // Convert Sprite to Texture2D
        Texture2D sourceTex = spriteImage.texture;

        // Resize to match canvas dimensions
        RenderTexture resizedRT = ResizeTextureToCanvas(sourceTex, canvasProxy.renderTexture.width, canvasProxy.renderTexture.height);

        // Paint the resized texture onto the canvas RenderTexture
        Graphics.Blit(resizedRT, canvasProxy.renderTexture);

        // Cleanup
        RenderTexture.ReleaseTemporary(resizedRT);
    }

    public void PaintGIF(CanvasProxyController canvasProxy, string gifFileName)
    {
        string gifPath = Path.Combine(Application.streamingAssetsPath, gifFileName);
        GIFManager.Instance.LoadGif(canvasProxy, gifPath);
    }

    private RenderTexture ResizeTextureToCanvas(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        rt.filterMode = FilterMode.Bilinear;

        Graphics.Blit(source, rt); // GPU rescale

        return rt;
    }

    public void PaintColor(CanvasProxyController canvasProxy, RaycastHit hit, int currentBrushSize, Color currentColor, PointerPhase pointerPhase)
    {
        if (pointerPhase == PointerPhase.Started)
        {
            // Prepare First Task
            PaintColorTask newTask = new PaintColorTask();

            newTask.pixelCoords = ConvertHitLocationToPixelCoords(canvasProxy, hit);
            
            useBrushSize = currentBrushSize;
            useColor = currentColor;

            // Add Task to List
            paintColorTaskList.Add(newTask);

            lastProperPixelCoords = newTask.pixelCoords;
            lastCanvasProxyController = canvasProxy;
        }
        else if (pointerPhase == PointerPhase.Pressed || pointerPhase == PointerPhase.Ended)
        {
            // Pointer started at a different Canvas from current one, ignore all tasks until finger is lifted and replaced
            if (lastCanvasProxyController != canvasProxy)
                return;

            // Interpolate between Last Pixel in Task List and this Pixel
            Vector2Int oldestPixelCoords = lastProperPixelCoords;
            Vector2Int latestPixelCoords = ConvertHitLocationToPixelCoords(canvasProxy, hit);
            List<Vector2Int> interpolatedPixels = ComputeInterpolatedPixels(oldestPixelCoords, latestPixelCoords);

            // Add all Pixels to Task List
            for (int i = 0; i < interpolatedPixels.Count; i++)
            {
                Vector2Int pixelCoords = interpolatedPixels[i];

                PaintColorTask newTask = new PaintColorTask();

                newTask.pixelCoords = pixelCoords;

                paintColorTaskList.Add(newTask);
            }

            // Add Latest Pixel
            PaintColorTask lastTask = new PaintColorTask();

            lastTask.pixelCoords = latestPixelCoords;

            paintColorTaskList.Add(lastTask);

            lastProperPixelCoords = latestPixelCoords;

            if (pointerPhase == PointerPhase.Ended)
            {
                shouldQueueAllTasks = false; // Flush all Tasks to Queue
                lastCanvasProxyController = null;
            }
        }
    }


    Vector2Int ConvertHitLocationToPixelCoords(CanvasProxyController canvasProxy, RaycastHit hit)
    {
        float halfWidth = canvasProxy.refPlaneData.halfSize.x;
        float halfHeight = canvasProxy.refPlaneData.halfSize.y;

        // Transform world hit into local hit
        Vector3 localHit = canvasProxy.transform.InverseTransformPoint(hit.point);

        float u = (localHit.x + halfWidth) / (2 * halfWidth);
        float v = (localHit.z + halfHeight) / (2 * halfHeight);

        RenderTexture rt = canvasProxy.renderTexture;
        int px = Mathf.FloorToInt(u * rt.width);
        int py = Mathf.FloorToInt(v * rt.height);

        return new Vector2Int(px, py);
    }

    List<Vector2Int> ComputeInterpolatedPixels(Vector2Int p1, Vector2Int p2)
    {
        List<Vector2Int> pixelsList = new List<Vector2Int>();

        for (int i = 1; i <= numInterpolatedPixels; i++)
        {
            float t = (float)i / (numInterpolatedPixels + 1);

            float x = Mathf.Lerp(p1.x, p2.x, t);
            float y = Mathf.Lerp(p1.y, p2.y, t);

            pixelsList.Add(new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y)));
        }

        return pixelsList;
    }

    public void Update()
    {
        // Queue Paint Jobs if any
        if (paintColorTaskList.Count > 0)
        {
            int countJobs = 0;
            for (int i = 0; i < paintColorTaskList.Count; i++)
            {
                ++countJobs;

                // Deploy Tasks to Paint Queue
                PaintColorTask queuedTask = paintColorTaskList[i];
                paintTasksQueue.Add(queuedTask);

                // Only Queue Batch Size Jobs unless all Tasks should be Queued
                if (countJobs >= maxPaintBatchSize && !shouldQueueAllTasks)
                {
                    break;
                }
            }

            // Remove Queued Tasks
            paintColorTaskList.RemoveRange(0, countJobs);

            // Reset Flush Flag
            if (shouldQueueAllTasks)
            {
                shouldQueueAllTasks = false;
            }
        }

        // Collect Queued Jobs for Dispatch
        if (paintTasksQueue.Count > 0)
        {
            DispatchPaintJobs();
        }    
    }

    void DispatchPaintJobs()
    {
        int[] pixelArr = new int[maxShaderPixelCount * 2];

        // Capture all Pixels to Paint this Frame
        int countJobs = 0;
        for (int i = 0; i < paintTasksQueue.Count; i++)
        {
            pixelArr[i * 2 + 0] = paintTasksQueue[i].pixelCoords.x;
            pixelArr[i * 2 + 1] = paintTasksQueue[i].pixelCoords.y;

            ++countJobs;

            if (countJobs >= maxShaderPixelCount)
            {
                break;
            }
        }

        // Remove Completed Tasks
        paintTasksQueue.RemoveRange(0, countJobs);

        // Dispatch the Shader
        // Set the render target
        batchedPaintShader.SetTexture(paintShaderKernel, "Result", lastCanvasProxyController.renderTexture);
        batchedPaintShader.SetInts("paintPixelArr", pixelArr);
        batchedPaintShader.SetInt("numOfPixels", countJobs);

        batchedPaintShader.SetInt("brushSize", useBrushSize);
        batchedPaintShader.SetVector("color", useColor);

        int groupsX = Mathf.CeilToInt((useBrushSize * 2) / 8.0f);
        int groupsY = Mathf.CeilToInt((useBrushSize * 2) / 8.0f);
        batchedPaintShader.Dispatch(paintShaderKernel, groupsX, groupsY, 1);
    }
}
