using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class GIFAnimation
{
    public int currentFrame = 0;
    public float timer = 0f;
    public float frameDuration = 2.0f / 30.0f;

    public List<UniGif.GifTexture> frames = new List<UniGif.GifTexture>();
    public int loopCount = 0;

    public bool IsLoaded => frames.Count > 0;

    public void Update(CanvasProxyController canvasProxy)
    {
        if (!IsLoaded) return;

        timer += Time.deltaTime;

        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame = (currentFrame + 1) % frames.Count;
        }

        Texture2D renderFrame = frames[currentFrame].m_texture2d;

        // Resize to match canvas dimensions
        RenderTexture resizedRT = RenderTexture.GetTemporary(canvasProxy.renderTexture.width, canvasProxy.renderTexture.height, 0, RenderTextureFormat.ARGB32);
        resizedRT.filterMode = FilterMode.Bilinear;
        Graphics.Blit(renderFrame, resizedRT);

        // Paint the resized texture onto the canvas RenderTexture
        Graphics.Blit(resizedRT, canvasProxy.renderTexture);

        // Cleanup
        RenderTexture.ReleaseTemporary(resizedRT);
    }
}
