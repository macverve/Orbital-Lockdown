using UnityEngine;
using TMPro;

public class ResolutionSettingsScript : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    private const string ResolutionWidthKey = "ResolutionWidth";
    private const string ResolutionHeightKey = "ResolutionHeight";

    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;

            if (!options.Contains(option))
            {
                options.Add(option);
            }

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = options.IndexOf(option);
            }
        }

        resolutionDropdown.AddOptions(options);

        int savedWidth = PlayerPrefs.GetInt(ResolutionWidthKey, Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt(ResolutionHeightKey, Screen.currentResolution.height);

        for (int i = 0; i < options.Count; i++)
        {
            if (options[i] == savedWidth + " x " + savedHeight)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();

        ApplyResolutionFromText(options[currentResolutionIndex]);
    }

    public void SetResolution(int resolutionIndex)
    {
        string selectedResolution = resolutionDropdown.options[resolutionIndex].text;
        ApplyResolutionFromText(selectedResolution);
    }

    private void ApplyResolutionFromText(string resolutionText)
    {
        string[] parts = resolutionText.Split('x');

        int width = int.Parse(parts[0].Trim());
        int height = int.Parse(parts[1].Trim());

        Screen.SetResolution(width, height, Screen.fullScreen);

        PlayerPrefs.SetInt(ResolutionWidthKey, width);
        PlayerPrefs.SetInt(ResolutionHeightKey, height);
        PlayerPrefs.Save();
    }
}