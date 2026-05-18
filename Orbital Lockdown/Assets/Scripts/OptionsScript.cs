using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class OptionsScript : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void BackButtonClick()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void SetSoundVolume(float volume)
    {
        float convertedVolume = Mathf.Log10(volume / 100) * 20;
        audioMixer.SetFloat("Volume", convertedVolume);
    }
}
