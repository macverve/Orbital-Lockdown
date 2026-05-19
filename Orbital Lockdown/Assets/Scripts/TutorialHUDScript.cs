using UnityEngine;
using System.Collections;

public class TutorialHUDScript : MonoBehaviour
{
    public GameObject tasksOverlay;
    public GameObject controlsOverlay;
    public float controlsDisplayTime = 30f;

    private void Start()
    {
        tasksOverlay.SetActive(true);
        controlsOverlay.SetActive(true);

        StartCoroutine(HideControlsAfterDelay());
    }

    private IEnumerator HideControlsAfterDelay()
    {
        yield return new WaitForSecondsRealtime(controlsDisplayTime);

        controlsOverlay.SetActive(false);
    }

    public void HideControlsNow()
    {
        controlsOverlay.SetActive(false);
    }
}