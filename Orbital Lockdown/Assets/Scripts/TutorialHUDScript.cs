using UnityEngine;
using System.Collections;

public class TutorialHUDScript : MonoBehaviour
{
    public GameObject tutorialHUDPanel;
    public GameObject tasksOverlay;
    public GameObject controlsOverlay;

    public float controlsDisplayTime = 30f;

    private Coroutine hideControlsCoroutine;

    private void Start()
    {
        HideTutorialHUD();
    }

    public void ShowTutorialHUD()
    {
        tutorialHUDPanel.SetActive(true);
        tasksOverlay.SetActive(true);
        controlsOverlay.SetActive(true);

        if (hideControlsCoroutine != null)
        {
            StopCoroutine(hideControlsCoroutine);
        }

        hideControlsCoroutine = StartCoroutine(HideControlsAfterDelay());
    }

    public void HideTutorialHUD()
    {
        if (tutorialHUDPanel != null)
        {
            tutorialHUDPanel.SetActive(false);
        }
    }

    private IEnumerator HideControlsAfterDelay()
    {
        yield return new WaitForSecondsRealtime(controlsDisplayTime);

        controlsOverlay.SetActive(false);
    }
}