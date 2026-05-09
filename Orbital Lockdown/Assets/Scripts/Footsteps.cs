using UnityEngine;

public class PlaySoundOnceOnTrigger : MonoBehaviour
{
    public AudioSource audioSource;
    public string playerTag = "Player";

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;

        if (other.CompareTag(playerTag))
        {
            if (audioSource != null)
            {
                audioSource.Play();
                hasPlayed = true;
            }
        }
    }
}