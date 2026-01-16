using UnityEngine;
using UnityEngine.UI;

public class UICanvasResizeController : MonoBehaviour
{
    // Selected Canvas Proxy
    [HideInInspector] public GameObject selectedCanvasProxy;
    [HideInInspector] public CanvasProxyController selectedCanvasProxyController;

    [SerializeField] Slider widthSlider;
    [SerializeField] Slider heightSlider;

    // Label
    [SerializeField] TMPro.TMP_Text canvasSelectedLabel;


    float originalWidth;
    float originalHeight;

    float currentWidthFactor = 1.0f;
    float currentHeightFactor = 1.0f;

    private void OnEnable()
    {
        selectedCanvasProxy = null;
        selectedCanvasProxyController = null;

        widthSlider.value = 1.0f;
        heightSlider.value = 1.0f;

        widthSlider.onValueChanged.RemoveAllListeners();
        heightSlider.onValueChanged.RemoveAllListeners();

        canvasSelectedLabel.text = "Nothing Selected!";
    }

    public void SetCanvas(GameObject canvasProxyGameObject)
    {
        // Don't Reset Stuff if Same Object was Passed
        if (selectedCanvasProxy == canvasProxyGameObject) return;

        selectedCanvasProxy = canvasProxyGameObject;
        selectedCanvasProxyController = canvasProxyGameObject.GetComponent<CanvasProxyController>();
        
        originalWidth = selectedCanvasProxyController.refPlaneData.halfSize.x * 2.0f;
        originalHeight = selectedCanvasProxyController.refPlaneData.halfSize.y * 2.0f;


        widthSlider.value = 1.0f;
        heightSlider.value = 1.0f;

        widthSlider.onValueChanged.AddListener(WidthChanged);
        heightSlider.onValueChanged.AddListener(HeightChanged);

        canvasSelectedLabel.text = "Canvas Selected!";
    }

    void ResizeCanvas()
    {
        if (!selectedCanvasProxyController) return;

        float newWidth = currentWidthFactor * originalWidth;
        float newHeight = currentHeightFactor * originalHeight;

        selectedCanvasProxyController.Resize(newWidth, newHeight);
    }

    void WidthChanged(float widthFactor) { currentWidthFactor = widthFactor; ResizeCanvas(); }
    void HeightChanged(float heightFactor) { currentHeightFactor = heightFactor; ResizeCanvas(); }

}
