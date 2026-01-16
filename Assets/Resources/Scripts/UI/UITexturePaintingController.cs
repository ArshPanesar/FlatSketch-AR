using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITexturePaintingController : MonoBehaviour
{
    public event Action<GameObject> OnImageTextureSelected;

    public void OnImageSelected(GameObject imageGameObject)
    {
        Image imageComp = imageGameObject.GetComponent<Image>();
        if (imageComp != null)
        {
            OnImageTextureSelected?.Invoke(imageGameObject);
        }
    }
}
