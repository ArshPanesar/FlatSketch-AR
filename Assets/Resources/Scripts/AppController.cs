using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public enum PointerPhase
{
    None,
    Started,
    Pressed, // Holding/Continuous Touch
    Ended
};

// Manages all Systems during the Lifetime of the Application
// * Responsible for Running/Controlling Systems Regularly or on Event Invokes.
// * Responsible for Supplying Data/References and Subscribing to Events for certain Systems (if can't be done by the Systems themselves).
public class AppController : MonoBehaviour
{
    // Required References to Systems for the App
    [SerializeField] private ARSystemsController refARSystemsController;
    [SerializeField] private UIMasterController refUIMasterController;
    Camera mainCamera;

    // Required Systems for the App
    PlaneDataManager planeDataManager;
    CanvasProxyManager canvasProxyManager;
    PaintingSystem paintingSystem;

    public GameObject debugGameObject;

    // Flags
    private bool isEnvironmentScanningComplete = false;

    // Input
    PointerPhase currentPointerPhase;

    void Start()
    {
        // Application starts with Plane Detection Enabled.
        // Only after Disabling Plane Detection will the Canvas and Painting Systems be Activated.
        isEnvironmentScanningComplete = false;

        // Ensure all Required Systems are provided
        Debug.Assert(refARSystemsController != null);
        Debug.Assert(refUIMasterController != null);

        mainCamera = refARSystemsController.refCamera;

        // Subscribe to Necessary Events
        refARSystemsController.OnPlaneDetectionComplete += OnEnvironmentScanningComplete;
        
        planeDataManager = new PlaneDataManager();
        canvasProxyManager = new CanvasProxyManager();
        paintingSystem = new PaintingSystem();

        currentPointerPhase = PointerPhase.None;
    }

    void Update()
    {
        if (isEnvironmentScanningComplete)
        {
            // Take both Mouse and Touch as Input
            Vector2 inputPos = Vector2.zero;
            bool inputDetected = false;
            currentPointerPhase = PointerPhase.None;

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                int fingerId = (int)Touchscreen.current.primaryTouch.touchId.ReadValue();

                // Don't Consider UI Selection as Touch/Click
                if (!EventSystem.current.IsPointerOverGameObject(fingerId))
                {
                    inputPos = Touchscreen.current.primaryTouch.position.ReadValue();

                    // Update Pointer Phase
                    currentPointerPhase = GetPointerPhase();

                    inputDetected = true;
                }

            }
            else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                // Don't Consider UI Selection as Touch/Click
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    inputPos = Mouse.current.position.ReadValue();

                    // Update Pointer Phase
                    currentPointerPhase = GetPointerPhase();

                    inputDetected = true;
                }
            }
            else
            {
                inputPos = Vector2.zero;
            }

            if (inputDetected)
            {
                // Perform a Raycast
                Ray ray = mainCamera.ScreenPointToRay(inputPos);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Check if a Canvas Proxy was Hit
                    CanvasProxyController proxy = hit.collider.GetComponent<CanvasProxyController>();
                    if (proxy != null)
                    {
                        // Execute based on Current UI Mode
                        if (refUIMasterController.GetCurrentMode() == UIMasterController.Modes.Painting)
                        {
                            if (refUIMasterController.GetPaintingType() == UIMasterController.PaintingType.Color)
                            {
                                // Paint with Selected Color and Brush Size
                                int brushSize = refUIMasterController.GetCurrentBrushSize();
                                Color color = refUIMasterController.GetCurrentPaintingColor();
                                paintingSystem.PaintColor(proxy, hit, brushSize, color, currentPointerPhase);
                            }
                            else if (refUIMasterController.GetPaintingType() == UIMasterController.PaintingType.Texture)
                            {
                                PaintStaticImageOrGIF(proxy, refUIMasterController.GetSelectedImage());
                            }
                        }
                        else if (refUIMasterController.GetCurrentMode() == UIMasterController.Modes.Canvas_Editing)
                        {
                            // Edit Canvas Details
                            refUIMasterController.SetCanvasEditingParams(proxy.gameObject);
                        }
                        else if (refUIMasterController.GetCurrentMode() == UIMasterController.Modes.Canvas_Resizing)
                        {
                            // Edit Canvas Details
                            refUIMasterController.SetCanvasResizingParams(proxy.gameObject);
                        }
                        else if (refUIMasterController.GetCurrentMode() == UIMasterController.Modes.Canvas_Duplicating)
                        {
                            canvasProxyManager.DuplicateFromProxyController(proxy);
                            // Force Reset UI Mode
                            refUIMasterController.ToggleCanvasDuplicatingMode();
                        }
                    }
                }
            }
            
            // Update Systems
            paintingSystem.Update();
        }
    }

    void PaintStaticImageOrGIF(CanvasProxyController proxy, GameObject imageGameObject)
    {
        // Check if its a GIF
        // All GameObjects representing a GIF will be appended by "GIF"
        if (imageGameObject.name.Contains("GIF"))
        {
            // Extract File Name
            Image imageComp = imageGameObject.GetComponent<Image>();
            string spriteName = imageComp.sprite.name + ".gif";
            
            paintingSystem.PaintGIF(proxy, spriteName);
        }
        else
        {
            // Paint Texture on Canvas
            paintingSystem.PaintTexture(proxy, imageGameObject);
        }
    }

    PointerPhase GetPointerPhase()
    {
        if (Touchscreen.current != null)
        {
            var t = Touchscreen.current.primaryTouch;
            if (t.press.wasPressedThisFrame) return PointerPhase.Started;
            if (t.press.wasReleasedThisFrame) return PointerPhase.Ended;
            if (t.press.isPressed) return PointerPhase.Pressed;
        }

        if (Mouse.current != null)
        {
            var m = Mouse.current.leftButton;
            if (m.wasPressedThisFrame) return PointerPhase.Started;
            if (m.wasReleasedThisFrame) return PointerPhase.Ended;
            if (m.isPressed) return PointerPhase.Pressed;
        }

        return PointerPhase.None;
    }

    void OnEnvironmentScanningComplete()
    {
        isEnvironmentScanningComplete = true;

        // Generate Plane Data from ARPlaneManager
        planeDataManager.GenerateFromARPlaneManager(refARSystemsController.refPlaneManager);

        // Generate Canvases
        canvasProxyManager.GenerateProxiesFromPlaneData(planeDataManager);
    }
}
