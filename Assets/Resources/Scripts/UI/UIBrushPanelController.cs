using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIBrushPanelController : MonoBehaviour
{
    public TMP_Text currentBrushSizeText;

    public Slider brushSizeSlider;

    public int currentBrushSize;

    void Start()
    {
        currentBrushSize = (int)brushSizeSlider.value;

        currentBrushSizeText.text = currentBrushSize.ToString() + " px";
    }

    public void OnSliderValueChanged()
    {
        currentBrushSize = (int)brushSizeSlider.value;

        currentBrushSizeText.text = currentBrushSize.ToString() + " px";
    }
}
