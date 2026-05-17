using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Camera))]
public class LowResCameraLook : MonoBehaviour
{
    [Range(10f, 100f)]
    public float resolutionPercent = 40f;

    private HDAdditionalCameraData hdCam;

    private void Awake()
    {
        hdCam = GetComponent<HDAdditionalCameraData>();

        if (hdCam != null)
        {
            hdCam.allowDynamicResolution = true;
        }

        ApplyResolutionScale();
    }

    private void OnEnable()
    {
        ApplyResolutionScale();
    }

    private void OnValidate()
    {
        resolutionPercent = Mathf.Clamp(resolutionPercent, 10f, 100f);

        if (enabled)
        {
            ApplyResolutionScale();
        }
    }

    private void ApplyResolutionScale()
    {
        DynamicResolutionHandler.SetDynamicResScaler(
            () => resolutionPercent,
            DynamicResScalePolicyType.ReturnsPercentage
        );
    }

    private void OnDisable()
    {
        DynamicResolutionHandler.SetDynamicResScaler(
            () => 100f,
            DynamicResScalePolicyType.ReturnsPercentage
        );
    }
}