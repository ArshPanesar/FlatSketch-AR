using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GIFManager : MonoBehaviour
{
    private Dictionary<CanvasProxyController, GIFAnimation> gifs =
        new Dictionary<CanvasProxyController, GIFAnimation>();

    public static GIFManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void RemoveGIF(CanvasProxyController proxyController)
    {
        gifs.Remove(proxyController);
    }

    public void LoadGif(CanvasProxyController canvas, string filePath)
    {
        GIFAnimation anim = new GIFAnimation();
        gifs[canvas] = anim;

        StartCoroutine(LoadGifCoroutine(anim, filePath));
    }

    private IEnumerator LoadGifCoroutine(GIFAnimation anim, string filePath)
    {
        byte[] bytes;

#if UNITY_EDITOR || UNITY_STANDALONE
        // Direct file read works in Editor/Standalone
        bytes = System.IO.File.ReadAllBytes(filePath);
#elif UNITY_ANDROID || UNITY_IOS
        // Mobile platforms: We need to use UnityWebRequest
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load GIF: " + www.error);
            yield break;
        }

        bytes = www.downloadHandler.data;
#else
        Debug.LogError("Platform not supported for GIF loading");
        yield break;
#endif

        // Now use UniGif
        yield return UniGif.GetTextureListCoroutine(
            bytes,
            (List<UniGif.GifTexture> texList, int loopCount, int width, int height) =>
            {
                anim.frames = texList;
                anim.loopCount = loopCount;
            }
        );
    }


    private void Update()
    {
        foreach (var pair in gifs)
        {
            CanvasProxyController canvas = pair.Key;
            GIFAnimation anim = pair.Value;

            anim.Update(canvas);
        }
    }
}
