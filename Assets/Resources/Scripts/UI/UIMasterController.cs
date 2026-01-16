using UnityEngine;
using UnityEngine.UI;

public class UIMasterController : MonoBehaviour
{
    public enum Modes
    {
        None,
        Painting,
        Canvas_Editing,
        Canvas_Resizing,
        Canvas_Duplicating
    };

    public enum PaintingType 
    {
        None,
        Color,
        Texture
    };

    Modes currentMode = Modes.None;
    PaintingType currentPaintingType = PaintingType.None;

    // Painting Mode
    public GameObject paintingPanel;
    public GameObject colorPaintingPanel;
    public GameObject texturePaintingPanel;

    // Color Painting
    public UIColorPickerController colorPickerController;
    public UIBrushPanelController brushPanelController;

    // Texture Painting
    public GameObject selectedImage;

    // Canvas Editing Mode
    public GameObject canvasEditingPanel;
    public UICanvasEditingController canvasEditingController;

    // Canvas Resizing Mode
    public GameObject canvasResizingPanel;
    public UICanvasResizeController canvasResizingController;

    // Canvas Resizing Mode
    public GameObject canvasDuplicatingPanel;


    void Start()
    {
        currentMode = Modes.None;

        texturePaintingPanel.GetComponent<UITexturePaintingController>().OnImageTextureSelected += OnImageTextureSelected;
    }

    public void TogglePaintingMode()
    {
        if (currentMode == Modes.Painting)
        {
            currentMode = Modes.None;
            currentPaintingType = PaintingType.None;
            paintingPanel.SetActive(false);
        }
        else
        {
            currentMode = Modes.Painting;
            currentPaintingType = PaintingType.Color;
            paintingPanel.SetActive(true);

            OpenColorPaintingPanel();
        }
    }

    public void ToggleCanvasEditingMode()
    {
        if (currentMode == Modes.Canvas_Editing)
        {
            currentMode = Modes.None;
            canvasEditingPanel.SetActive(false);
        }
        else
        {
            currentMode = Modes.Canvas_Editing;
            canvasEditingPanel.SetActive(true);
        }
    }

    public void ToggleCanvasResizingMode()
    {
        if (currentMode == Modes.Canvas_Resizing)
        {
            currentMode = Modes.None;
            canvasResizingPanel.SetActive(false);
        }
        else
        {
            currentMode = Modes.Canvas_Resizing;
            canvasResizingPanel.SetActive(true);
        }
    }

    public void ToggleCanvasDuplicatingMode()
    {
        if (currentMode == Modes.Canvas_Duplicating)
        {
            currentMode = Modes.None;
            canvasDuplicatingPanel.SetActive(false);
        }
        else
        {
            currentMode = Modes.Canvas_Duplicating;
            canvasDuplicatingPanel.SetActive(true);
        }
    }

    public void OpenColorPaintingPanel()
    {
        if (currentMode == Modes.Painting)
        {
            colorPaintingPanel.SetActive(true);
            texturePaintingPanel.SetActive(false);
            currentPaintingType = PaintingType.Color;
        }
    }

    public void OpenTexturePaintingPanel()
    {
        if (currentMode == Modes.Painting)
        {
            colorPaintingPanel.SetActive(false);
            texturePaintingPanel.SetActive(true);
            currentPaintingType = PaintingType.Texture;
        }
    }

    void OnImageTextureSelected(GameObject imageGameObject)
    {
        selectedImage = imageGameObject;
    }

    public GameObject GetSelectedImage()
    {
        return selectedImage;
    }

    public Modes GetCurrentMode()
    {
        return currentMode;
    }

    public PaintingType GetPaintingType()
    {
        return currentPaintingType;
    }

    public Color GetCurrentPaintingColor()
    {
        return colorPickerController.GetRGBColor();
    }

    public int GetCurrentBrushSize()
    {
        return brushPanelController.currentBrushSize;
    }

    public void SetCanvasEditingParams(GameObject canvasProxy)
    {
        canvasEditingController.SetCanvas(canvasProxy);
    }

    public void SetCanvasResizingParams(GameObject canvasProxy)
    {
        canvasResizingController.SetCanvas(canvasProxy);
    }
}
