using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FlickerSoundFromLight : MonoBehaviour
{
    [Header("Light To Watch")]
    [SerializeField] private Light targetLight;

    [Header("Flicker Sounds")]
    [SerializeField] private AudioClip[] flickerSounds;
    [SerializeField] private float volume = 0.25f;
    [SerializeField] private bool randomizePitch = true;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.85f, 1.15f);

    [Header("Detection")]
    [SerializeField] private float intensityDropThreshold = 0.35f;
    [SerializeField] private float minimumTimeBetweenSounds = 0.12f;
    [SerializeField] private bool playOnlyOnDrops = true;

    private AudioSource audioSource;
    private float previousIntensity;
    private bool previousEnabled;
    private float lastSoundTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }

        if (targetLight != null)
        {
            previousIntensity = targetLight.intensity;
            previousEnabled = targetLight.enabled;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
    }

    private void Update()
    {
        if (targetLight == null || flickerSounds == null || flickerSounds.Length == 0)
        {
            return;
        }

        float currentIntensity = targetLight.enabled ? targetLight.intensity : 0f;

        bool lightTurnedOff = previousEnabled && !targetLight.enabled;
        bool lightTurnedOn = !previousEnabled && targetLight.enabled;

        float intensityDrop = previousIntensity - currentIntensity;
        bool strongDrop = intensityDrop >= intensityDropThreshold;

        if (Time.time - lastSoundTime < minimumTimeBetweenSounds)
        {
            previousIntensity = currentIntensity;
            previousEnabled = targetLight.enabled;
            return;
        }

        if (lightTurnedOff || strongDrop || (!playOnlyOnDrops && lightTurnedOn))
        {
            PlayFlickerSound();
        }

        previousIntensity = currentIntensity;
        previousEnabled = targetLight.enabled;
    }

    private void PlayFlickerSound()
    {
        AudioClip clip = flickerSounds[Random.Range(0, flickerSounds.Length)];

        if (clip == null)
        {
            return;
        }

        if (randomizePitch)
        {
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        }
        else
        {
            audioSource.pitch = 1f;
        }

        audioSource.PlayOneShot(clip, volume);
        lastSoundTime = Time.time;
    }
}