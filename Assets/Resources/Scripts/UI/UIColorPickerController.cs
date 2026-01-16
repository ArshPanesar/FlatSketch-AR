using UnityEngine;
using UnityEngine.UI;

public class UIColorPickerController : MonoBehaviour
{
    public float currentHue;
    public float currentSat;
    public float currentVal;

    [SerializeField]
    private RawImage hueImage;
    [SerializeField]
    private RawImage satValImage;
    [SerializeField]
    private RawImage outputImage;

    [SerializeField]
    private Slider hueSlider;

    private Texture2D hueTexture;
    private Texture2D svTexture;
    private Texture2D outputTexture;


    private void Start()
    {
        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();

        UpdateOutputImage();
    }

    private void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        for (int i = 0; i < hueTexture.height; ++i)
        {
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / (float)hueTexture.height, 1.0f, 1.0f));
        }

        hueTexture.Apply();
        currentHue = 0.0f;

        hueImage.texture = hueTexture;
    }

    private void CreateSVImage()
    {
        svTexture = new Texture2D(16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        for (int y = 0; y < svTexture.height; ++y)
        {
            for (int x = 0; x < svTexture.width; ++x)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / (float)svTexture.width, (float)y / (float)svTexture.height));
            }
        }

        svTexture.Apply();
        currentSat = 0.0f;
        currentVal = 0.0f;

        satValImage.texture = svTexture;
    }

    private void CreateOutputImage()
    {
        outputTexture = new Texture2D(8, 8);
        outputTexture.wrapMode = TextureWrapMode.Clamp;
        outputTexture.name = "OutputTexture";

        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);
        for (int y = 0; y < outputTexture.height; ++y)
        {
            for (int x = 0; x < outputTexture.width; ++x)
            {
                outputTexture.SetPixel(x, y, currentColor);
            }
        }

        outputTexture.Apply();

        outputImage.texture = outputTexture;
    }

    private void UpdateOutputImage()
    {
        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);
        for (int y = 0; y < outputTexture.height; ++y)
        {
            for (int x = 0; x < outputTexture.width; ++x)
            {
                outputTexture.SetPixel(x, y, currentColor);
            }
        }

        outputTexture.Apply();

        outputImage.texture = outputTexture;
    }

    public void SetSatVal(float sat, float val)
    {
        currentSat = sat;
        currentVal = val;

        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
        currentHue = hueSlider.value;

        for (int y = 0; y < svTexture.height; ++y)
        {
            for (int x = 0; x < svTexture.width; ++x)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / (float)svTexture.width, (float)y / (float)svTexture.height));
            }
        }

        svTexture.Apply();

        UpdateOutputImage();
    }

    public Color GetRGBColor()
    {
        return Color.HSVToRGB(currentHue, currentSat, currentVal);
    }
}
