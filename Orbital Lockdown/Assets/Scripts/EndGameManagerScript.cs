using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    public GameObject endGamePanel;
    public GameObject successImage;
    public GameObject deathImage;

    public AudioSource winAudioSource;
    public AudioSource deathAudioSource;

    public void ShowEscapeEnding()
    {
        endGamePanel.SetActive(true);
        successImage.SetActive(true);
        deathImage.SetActive(false);

        StopGameplay();

        if (winAudioSource != null)
        {
            winAudioSource.ignoreListenerPause = true;
            winAudioSource.Play();
        }
    }

    public void ShowDeathEnding()
    {
        endGamePanel.SetActive(true);
        successImage.SetActive(false);
        deathImage.SetActive(true);

        StopGameplay();

        if (deathAudioSource != null)
        {
            deathAudioSource.ignoreListenerPause = true;
            deathAudioSource.Play();
        }
    }

    private void StopGameplay()
    {
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        AudioListener.pause = true;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenuScene");
    }
}