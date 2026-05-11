using UnityEngine;

public class PlaySoundOnceOnTrigger : MonoBehaviour
{
    public AudioSource audioSource;
    public string playerTag = "Player";

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER HIT by: " + other.name);

        if (hasPlayed) return;

        bool isPlayer =
            other.CompareTag(playerTag) ||
            other.transform.root.CompareTag(playerTag);

        if (isPlayer)
        {
            Debug.Log("PLAYER DETECTED - PLAY SOUND");

            if (audioSource != null)
            {
                audioSource.Play();
                hasPlayed = true;
            }
            else
            {
                Debug.LogError("AudioSource is missing in PlaySoundOnceOnTrigger");
            }
        }
    }
}