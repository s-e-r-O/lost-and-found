using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelatedCamera : MonoBehaviour
{
    public enum PixelScreenMode { Resize, Scale }

    [System.Serializable]
    public struct ScreenSize
    {
        // Basically an integer Vector2 to store screen size information
        public int width;
        public int height;
    }

    public static PixelatedCamera main;

    [HideInInspector]
    public Camera renderCamera;
    [SerializeField] private Camera nonPixelatedCamera;
    private RenderTexture renderTexture;
    private int screenWidth, screenHeight;

    [Header("Screen scaling settings")]
    public PixelScreenMode mode;
    public ScreenSize targetScreenSize = new ScreenSize { width = 256, height = 144 };  // Only used with PixelScreenMode.Resize
    public uint screenScaleFactor = 1;  // Only used with PixelScreenMode.Scale

    public Vector2 ScreenScale = Vector2.one;

    [Header("Display")]
    public RawImage display;
    //[Header("Display")]
    //public MeshRenderer quad;

    private void Awake()
    {
        // Try to set as main pixel camera
        if (main == null) main = this;
    }

    private void Start()
    {
        // Initialize the system
        Init();
    }

    private void Update()
    {
        // Re initialize system if the screen has been resized
        if (CheckScreenResize()) Init();
    }

    public void Init()
    {

        // Initialize the camera and get screen size values
        if (!renderCamera) renderCamera = GetComponent<Camera>();
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        // Prevent any error
        if (screenScaleFactor < 1) screenScaleFactor = 1;
        if (targetScreenSize.width < 1) targetScreenSize.width = 1;
        if (targetScreenSize.height < 1) targetScreenSize.height = 1;

        // Calculate the render texture size
        int width = mode == PixelScreenMode.Resize ? (int)targetScreenSize.width : screenWidth / (int)screenScaleFactor;
        int height = mode == PixelScreenMode.Resize ? (int)targetScreenSize.height : screenHeight / (int)screenScaleFactor;

        ScreenScale = new Vector2(width / (float)screenWidth, height / (float)screenHeight);

        // Initialize the render texture
        renderTexture = new RenderTexture(width, height, 24)
        {
            filterMode = FilterMode.Point,
            antiAliasing = 1
        };

        // Set the render texture as the camera's output
        renderCamera.targetTexture = renderTexture;

        if (nonPixelatedCamera != null)
        {
            nonPixelatedCamera.targetTexture = renderTexture;
        }

        // Attaching texture to the display UI RawImage
        display.texture = renderTexture;

        //quad.material.mainTexture = renderTexture;
    }

    public bool CheckScreenResize()
    {
        // Check whether the screen has been resized
        return Screen.width != screenWidth || Screen.height != screenHeight;
    }
}