using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsScript : MonoBehaviour
{
    public void BackButtonClick()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
