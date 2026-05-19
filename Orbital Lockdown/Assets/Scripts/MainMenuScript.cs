using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsPanel;
    public AudioMixer audioMixer;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OptionsMenu()
    {
        mainMenu.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackButtonClick()
    {
        settingsPanel.SetActive(false);
        mainMenu.SetActive(true);
    }
        public void SetSoundVolume(float volume)
    {
        if (volume <= 0)
        {
            audioMixer.SetFloat("Volume", -80f);
            return;
        }

        float convertedVolume = Mathf.Log10(volume / 100f) * 20f;
        audioMixer.SetFloat("Volume", convertedVolume);
    }

    public void SetMusicVolume(float volume)
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