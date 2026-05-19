using UnityEngine;

public class ScreamTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource screamSource;
    [SerializeField] private bool playOnlyOnce = true;

    private bool hasPlayed = false;

    private void Reset()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }

        screamSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playOnlyOnce && hasPlayed)
        {
            return;
        }

        if (!other.transform.root.CompareTag("Player"))
        {
            return;
        }

        if (screamSource == null)
        {
            Debug.LogWarning("No AudioSource assigned to ScreamTrigger.");
            return;
        }

        screamSource.Play();
        hasPlayed = true;

        if (playOnlyOnce)
        {
            Collider triggerCollider = GetComponent<Collider>();
            if (triggerCollider != null)
            {
                triggerCollider.enabled = false;
            }
        }
    }
}