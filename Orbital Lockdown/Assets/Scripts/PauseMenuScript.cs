using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject pauseSettingsPanel;
    public AudioMixer audioMixer;

    private bool isPaused = false;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
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

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        pauseSettingsPanel.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        pauseSettingsPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;
        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenPauseSettings()
    {
        pauseMenuPanel.SetActive(false);
        pauseSettingsPanel.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        pauseSettingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
}