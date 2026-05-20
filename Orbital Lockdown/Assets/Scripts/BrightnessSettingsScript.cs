using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class BrightnessSettingsScript : MonoBehaviour
{
    public Volume globalVolume;
    public Slider brightnessSlider;

    private ColorAdjustments colorAdjustments;

    private const string BrightnessKey = "Brightness";

    private void Start()
    {
        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            float savedBrightness = PlayerPrefs.GetFloat(BrightnessKey, 0f);

            if (brightnessSlider != null)
            {
                brightnessSlider.SetValueWithoutNotify(savedBrightness);
            }

            ApplyBrightness(savedBrightness);
        }
        else
        {
            Debug.LogWarning("BrightnessSettingsScript could not find Color Adjustments on the assigned Global Volume.");
        }
    }

    public void SetBrightness(float brightness)
    {
        PlayerPrefs.SetFloat(BrightnessKey, brightness);
        PlayerPrefs.Save();

        ApplyBrightness(brightness);
    }

    private void ApplyBrightness(float brightness)
    {
        if (colorAdjustments == null)
        {
            return;
        }

        colorAdjustments.postExposure.overrideState = true;
        colorAdjustments.postExposure.value = brightness;
    }
}