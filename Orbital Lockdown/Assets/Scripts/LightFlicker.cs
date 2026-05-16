using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [Header("Main light")]
    [SerializeField] private Light targetLight;

    [Header("Progression")]
    [Tooltip("Time in seconds until the flicker reaches maximum intensity.")]
    [SerializeField] private float timeToMaxIntensity = 180f;

    [Header("Chance over time")]
    [Range(0f, 1f)]
    [SerializeField] private float startFlickerChance = 0.05f;

    [Range(0f, 1f)]
    [SerializeField] private float maxFlickerChance = 0.5f;

    [SerializeField] private float startCheckInterval = 2.5f;
    [SerializeField] private float maxCheckInterval = 0.4f;

    [Header("Flicker duration over time")]
    [SerializeField] private float startMinFlickerDuration = 0.04f;
    [SerializeField] private float startMaxFlickerDuration = 0.12f;

    [SerializeField] private float maxMinFlickerDuration = 0.12f;
    [SerializeField] private float maxMaxFlickerDuration = 0.5f;

    [Header("Flicker speed over time")]
    [SerializeField] private float startMinFlickerSpeed = 0.02f;
    [SerializeField] private float startMaxFlickerSpeed = 0.05f;

    [SerializeField] private float maxMinFlickerSpeed = 0.005f;
    [SerializeField] private float maxMaxFlickerSpeed = 0.025f;

    [Header("Intensity over time")]
    [Range(0f, 1f)]
    [SerializeField] private float startMinIntensityMultiplier = 0.7f;

    [Range(0f, 1f)]
    [SerializeField] private float maxMinIntensityMultiplier = 0f;

    [Range(0f, 1f)]
    [SerializeField] private float startMaxIntensityMultiplier = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float maxMaxIntensityMultiplier = 0.8f;

    [Header("Full shutoff chance over time")]
    [Range(0f, 1f)]
    [SerializeField] private float startFullShutoffChance = 0.05f;

    [Range(0f, 1f)]
    [SerializeField] private float maxFullShutoffChance = 0.45f;

    [Header("Other")]
    [SerializeField] private bool randomizeStartDelay = true;
    [SerializeField] private bool useUnscaledTime = false;

    private float baseIntensity;
    private bool isFlickering;

    private void Reset()
    {
        targetLight = GetComponent<Light>();
    }

    private void Awake()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }

        baseIntensity = targetLight.intensity;
    }

    private void OnEnable()
    {
        StartCoroutine(FlickerLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        if (targetLight != null)
        {
            targetLight.enabled = true;
            targetLight.intensity = baseIntensity;
        }
    }

    private IEnumerator FlickerLoop()
    {
        if (randomizeStartDelay)
        {
            yield return Wait(Random.Range(0f, startCheckInterval));
        }

        while (true)
        {
            float progress = GetProgress();

            float currentCheckInterval = Mathf.Lerp(startCheckInterval, maxCheckInterval, progress);
            float currentFlickerChance = Mathf.Lerp(startFlickerChance, maxFlickerChance, progress);

            if (!isFlickering && Random.value <= currentFlickerChance)
            {
                yield return StartCoroutine(DoFlicker(progress));
            }

            yield return Wait(currentCheckInterval);
        }
    }

    private IEnumerator DoFlicker(float progress)
    {
        isFlickering = true;

        float currentMinDuration = Mathf.Lerp(startMinFlickerDuration, maxMinFlickerDuration, progress);
        float currentMaxDuration = Mathf.Lerp(startMaxFlickerDuration, maxMaxFlickerDuration, progress);

        float currentMinSpeed = Mathf.Lerp(startMinFlickerSpeed, maxMinFlickerSpeed, progress);
        float currentMaxSpeed = Mathf.Lerp(startMaxFlickerSpeed, maxMaxFlickerSpeed, progress);

        float currentMinIntensity = Mathf.Lerp(startMinIntensityMultiplier, maxMinIntensityMultiplier, progress);
        float currentMaxIntensity = Mathf.Lerp(startMaxIntensityMultiplier, maxMaxIntensityMultiplier, progress);

        float currentFullShutoffChance = Mathf.Lerp(startFullShutoffChance, maxFullShutoffChance, progress);

        float duration = Random.Range(currentMinDuration, currentMaxDuration);
        float timer = 0f;

        while (timer < duration)
        {
            float step = Random.Range(currentMinSpeed, currentMaxSpeed);

            if (Random.value <= currentFullShutoffChance)
            {
                targetLight.enabled = false;
            }
            else
            {
                targetLight.enabled = true;
                float multiplier = Random.Range(currentMinIntensity, currentMaxIntensity);
                targetLight.intensity = baseIntensity * multiplier;
            }

            yield return Wait(step);
            timer += step;
        }

        targetLight.enabled = true;
        targetLight.intensity = baseIntensity;
        isFlickering = false;
    }

    private float GetProgress()
    {
        float elapsed = useUnscaledTime ? Time.unscaledTime : Time.timeSinceLevelLoad;

        if (timeToMaxIntensity <= 0f)
        {
            return 1f;
        }

        return Mathf.Clamp01(elapsed / timeToMaxIntensity);
    }

    private object Wait(float seconds)
    {
        if (useUnscaledTime)
        {
            return new WaitForSecondsRealtime(seconds);
        }

        return new WaitForSeconds(seconds);
    }
}