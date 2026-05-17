using System.Collections;
using UnityEngine;

public class DelayedPitchWander : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    [Header("Timing")]
    [SerializeField] private float delayBeforeVariation = 40f;
    [SerializeField] private float minHoldTime = 4f;
    [SerializeField] private float maxHoldTime = 9f;
    [SerializeField] private float minTransitionTime = 1.5f;
    [SerializeField] private float maxTransitionTime = 4f;

    [Header("Pitch ranges")]
    [SerializeField] private bool allowNegativePitches = true;
    [SerializeField] private float minNegativePitch = -2.2f;
    [SerializeField] private float maxNegativePitch = -1.05f;
    [SerializeField] private float minPositivePitch = 1.05f;
    [SerializeField] private float maxPositivePitch = 1.8f;

    [Header("Start pitch")]
    [SerializeField] private float startingPitch = 1f;

    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("DelayedPitchWander: No AudioSource found.", this);
            enabled = false;
            return;
        }

        audioSource.pitch = startingPitch;
        StartCoroutine(PitchRoutine());
    }

    private IEnumerator PitchRoutine()
    {
        yield return new WaitForSeconds(delayBeforeVariation);

        while (true)
        {
            float targetPitch = GetRandomPitchOutsideBlockedRange();
            float transitionTime = Random.Range(minTransitionTime, maxTransitionTime);

            yield return StartCoroutine(SmoothlyMoveToPitch(targetPitch, transitionTime));

            float holdTime = Random.Range(minHoldTime, maxHoldTime);
            yield return new WaitForSeconds(holdTime);
        }
    }

    private IEnumerator SmoothlyMoveToPitch(float targetPitch, float duration)
    {
        float startPitch = audioSource.pitch;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth step for softer movement
            t = t * t * (3f - 2f * t);

            audioSource.pitch = Mathf.Lerp(startPitch, targetPitch, t);
            yield return null;
        }

        audioSource.pitch = targetPitch;
    }

    private float GetRandomPitchOutsideBlockedRange()
    {
        if (allowNegativePitches)
        {
            bool useNegative = Random.value < 0.5f;

            if (useNegative)
            {
                return Random.Range(minNegativePitch, maxNegativePitch);
            }
        }

        return Random.Range(minPositivePitch, maxPositivePitch);
    }
}