using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasEditingController : MonoBehaviour
{
    // Selected Canvas Proxy
    [HideInInspector] public GameObject selectedCanvasProxy;
    [HideInInspector] public CanvasProxyController selectedCanvasProxyController;

    // Editing Mode Selection Flag
    bool isCanvasSelected = false;

    // Position Sliders
    [SerializeField] Slider xPositionSlider;
    [SerializeField] Slider yPositionSlider;
    [SerializeField] Slider zPositionSlider;

    // Rotation Sliders
    [SerializeField] Slider xRotationSlider;
    [SerializeField] Slider yRotationSlider;
    [SerializeField] Slider zRotationSlider;

    // Label
    [SerializeField] TMPro.TMP_Text canvasSelectedLabel;

    // Current Position and Rotation
    Vector3 currentPositionChange;
    Vector3 originalPosition;

    Vector3 currentRotationChange;
    Quaternion originalRotation;


    private void OnEnable()
    {
        selectedCanvasProxy = null;
        selectedCanvasProxyController = null;

        isCanvasSelected = false;

        xPositionSlider.value = 0;
        yPositionSlider.value = 0;
        zPositionSlider.value = 0;

        xRotationSlider.value = 0;
        yRotationSlider.value = 0;
        zRotationSlider.value = 0;

        xPositionSlider.onValueChanged.RemoveAllListeners();
        yPositionSlider.onValueChanged.RemoveAllListeners();
        zPositionSlider.onValueChanged.RemoveAllListeners();
        
        xRotationSlider.onValueChanged.RemoveAllListeners();
        yRotationSlider.onValueChanged.RemoveAllListeners();
        zRotationSlider.onValueChanged.RemoveAllListeners();

        canvasSelectedLabel.text = "Nothing Selected!";
    }

    public void SetCanvas(GameObject canvasProxyGameObject)
    {
        // Don't Reset Stuff if Same Object was Passed
        if (selectedCanvasProxy == canvasProxyGameObject) return;

        selectedCanvasProxy = canvasProxyGameObject;
        selectedCanvasProxyController = canvasProxyGameObject.GetComponent<CanvasProxyController>();
        isCanvasSelected = true;

        originalPosition = canvasProxyGameObject.transform.localPosition;
        originalRotation = canvasProxyGameObject.transform.localRotation;

        xPositionSlider.value = 0;
        yPositionSlider.value = 0;
        zPositionSlider.value = 0;

        xRotationSlider.value = 0;
        yRotationSlider.value = 0;
        zRotationSlider.value = 0;

        xPositionSlider.onValueChanged.AddListener(XPositionValueChanged);
        yPositionSlider.onValueChanged.AddListener(YPositionValueChanged);
        zPositionSlider.onValueChanged.AddListener(ZPositionValueChanged);

        xRotationSlider.onValueChanged.AddListener(XRotationValueChanged);
        yRotationSlider.onValueChanged.AddListener(YRotationValueChanged);
        zRotationSlider.onValueChanged.AddListener(ZRotationValueChanged);


        canvasSelectedLabel.text = "Canvas Selected!";
    }

    void UpdatePosition()
    {
        if (!selectedCanvasProxyController) return;

        selectedCanvasProxy.transform.localPosition = originalPosition + currentPositionChange;
    }

    void UpdateRotation()
    {
        if (!selectedCanvasProxyController) return;

        selectedCanvasProxy.transform.localRotation = originalRotation * Quaternion.Euler(currentRotationChange);
    }

    void XPositionValueChanged(float newValue) { currentPositionChange.x = newValue; UpdatePosition(); }
    void YPositionValueChanged(float newValue) { currentPositionChange.y = newValue; UpdatePosition(); }
    void ZPositionValueChanged(float newValue) { currentPositionChange.z = newValue; UpdatePosition(); }

    void XRotationValueChanged(float newValue) { currentRotationChange.x = newValue; UpdateRotation(); }
    void YRotationValueChanged(float newValue) { currentRotationChange.y = newValue; UpdateRotation(); }
    void ZRotationValueChanged(float newValue) { currentRotationChange.z = newValue; UpdateRotation(); }
}
