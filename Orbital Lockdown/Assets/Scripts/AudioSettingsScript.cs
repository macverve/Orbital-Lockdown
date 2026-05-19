using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsScript : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider soundSlider;
    public Slider musicSlider;

    private const string SoundVolumeKey = "SoundVolume";
    private const string MusicVolumeKey = "MusicVolume";

    void Start()
    {
        float savedSoundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, 70f);
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 70f);

        if (soundSlider != null)
        {
            soundSlider.SetValueWithoutNotify(savedSoundVolume);
        }

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(savedMusicVolume);
        }

        ApplySoundVolume(savedSoundVolume);
        ApplyMusicVolume(savedMusicVolume);
    }

    public void SetSoundVolume(float volume)
    {
        PlayerPrefs.SetFloat(SoundVolumeKey, volume);
        PlayerPrefs.Save();

        ApplySoundVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();

        ApplyMusicVolume(volume);
    }

    private void ApplySoundVolume(float volume)
    {
        if (volume <= 0)
        {
            audioMixer.SetFloat("Volume", -80f);
            return;
        }

        float convertedVolume = Mathf.Log10(volume / 100f) * 20f;
        audioMixer.SetFloat("Volume", convertedVolume);
    }

    private void ApplyMusicVolume(float volume)
    {
        if (volume <= 0)
        {
            audioMixer.SetFloat("MusicVolume", -80f);
            return;
        }

        float convertedVolume = Mathf.Log10(volume / 100f) * 20f;
        audioMixer.SetFloat("MusicVolume", convertedVolume);
    }
}