using UnityEngine;

public class DeathTimerScript : MonoBehaviour
{
    public EndGameManager endGameManager;

    [Tooltip("Time before death screen appears, in seconds.")]
    public float timeUntilDeath = 420f; // 7 minutes

    private bool timerActive = true;

    private void Update()
    {
        if (!timerActive)
        {
            return;
        }

        timeUntilDeath -= Time.deltaTime;

        if (timeUntilDeath <= 0f)
        {
            timerActive = false;

            if (endGameManager != null)
            {
                endGameManager.ShowDeathEnding();
            }
        }
    }

    public void StopTimer()
    {
        timerActive = false;
    }
}