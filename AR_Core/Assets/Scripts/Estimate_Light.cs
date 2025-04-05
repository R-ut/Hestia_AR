using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class Estimate_Light : MonoBehaviour
{
    public ARCameraManager ARCamera;
    public TMP_Text Brightness;
    private Light Light_Scene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Light_Scene = GetComponent<Light>();
    }

    private void OnEnable()
    {
        ARCamera.frameReceived += GetLightEstimation;
    }

    void OnDisable()
    {
        ARCamera.frameReceived -= GetLightEstimation;
    }

    void GetLightEstimation(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.mainLightColor.HasValue)
        {
            Brightness.text = $"Color_Value:{args.lightEstimation.mainLightColor.Value}";
            Light_Scene.color = args.lightEstimation.mainLightColor.Value;
            float average_brightness = 0.2126f * Light_Scene.color.r + 0.7152f * Light_Scene.color.g + 0.0722f * Light_Scene.color.b ;
            Brightness.text = average_brightness.ToString();
        }
    }
}
