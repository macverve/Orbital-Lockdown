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
        audioMixer.SetFloat("Volume", volume);
    }
}
