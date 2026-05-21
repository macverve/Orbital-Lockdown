using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAndStartBlackout : MonoBehaviour
{
    [Header("Pickup Sounds")]
    [SerializeField] private AudioClip[] pickupSounds;
    [SerializeField] private float volume = 1f;
    [SerializeField] private bool playRandomSound = false;
    [SerializeField] private bool playAs3DSound = true;

    [Header("Blackout Timing")]
    [SerializeField] private bool waitForSoundToFinish = true;
    [SerializeField] private float manualBlackoutDelayFromPickup = 0f;
    [SerializeField] private float blackoutDelayAfterSound = 0f;

    [Header("Lights To Turn Off")]
    [SerializeField] private Transform[] lightRootsToTurnOff;
    [SerializeField] private Light[] extraLightsToTurnOff;
    [SerializeField] private bool disableFlickerScripts = true;

    [Header("Other Things To Disable")]
    [SerializeField] private Behaviour[] componentsToDisableAfterSound;
    [SerializeField] private GameObject[] objectsToDeactivateAfterSound;
    [SerializeField] private Renderer[] renderersToDisableAfterSound;
    [SerializeField] private ParticleSystem[] particleSystemsToStopAfterSound;

    [Header("Death Screen After Blackout")]
    [SerializeField] private GameObject deathScreenObject;
    [SerializeField] private float deathScreenDelayAfterBlackout = 60f;
    [SerializeField] private bool pauseGameWhenDeathScreenAppears = true;
    [SerializeField] private bool showCursorOnDeathScreen = true;

    private bool hasStarted;
    private bool hasPlayed;

    private void Start()
    {
        hasStarted = true;

        if (deathScreenObject != null)
        {
            deathScreenObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StartPickupSoundAndBlackout();
    }

    private void OnDestroy()
    {
        StartPickupSoundAndBlackout();
    }

    private void StartPickupSoundAndBlackout()
    {
        if (!Application.isPlaying || !hasStarted || hasPlayed)
        {
            return;
        }

        AudioClip[] clipsToPlay = GetClipsToPlay();

        if (clipsToPlay.Length == 0)
        {
            return;
        }

        hasPlayed = true;

        GameObject runnerObject = new GameObject("Pickup Sound And Blackout Runner");
        runnerObject.transform.position = transform.position;

        PickupSoundBlackoutRunner runner = runnerObject.AddComponent<PickupSoundBlackoutRunner>();

        runner.StartSequence(
            clipsToPlay,
            volume,
            playAs3DSound,
            waitForSoundToFinish,
            manualBlackoutDelayFromPickup,
            blackoutDelayAfterSound,
            GetLightsToTurnOff(),
            GetFlickerScriptsToDisable(),
            componentsToDisableAfterSound,
            objectsToDeactivateAfterSound,
            renderersToDisableAfterSound,
            particleSystemsToStopAfterSound,
            deathScreenObject,
            deathScreenDelayAfterBlackout,
            pauseGameWhenDeathScreenAppears,
            showCursorOnDeathScreen
        );
    }

    private AudioClip[] GetClipsToPlay()
    {
        List<AudioClip> validClips = new List<AudioClip>();

        foreach (AudioClip clip in pickupSounds)
        {
            if (clip != null)
            {
                validClips.Add(clip);
            }
        }

        if (validClips.Count == 0)
        {
            return new AudioClip[0];
        }

        if (playRandomSound)
        {
            return new AudioClip[] { validClips[Random.Range(0, validClips.Count)] };
        }

        return validClips.ToArray();
    }

    private Light[] GetLightsToTurnOff()
    {
        HashSet<Light> lights = new HashSet<Light>();

        foreach (Transform root in lightRootsToTurnOff)
        {
            if (root == null)
            {
                continue;
            }

            Light[] childLights = root.GetComponentsInChildren<Light>(true);

            foreach (Light childLight in childLights)
            {
                if (childLight != null)
                {
                    lights.Add(childLight);
                }
            }
        }

        foreach (Light extraLight in extraLightsToTurnOff)
        {
            if (extraLight != null)
            {
                lights.Add(extraLight);
            }
        }

        return new List<Light>(lights).ToArray();
    }

    private Behaviour[] GetFlickerScriptsToDisable()
    {
        if (!disableFlickerScripts)
        {
            return new Behaviour[0];
        }

        HashSet<Behaviour> flickerScripts = new HashSet<Behaviour>();

        foreach (Transform root in lightRootsToTurnOff)
        {
            if (root == null)
            {
                continue;
            }

            MonoBehaviour[] behaviours = root.GetComponentsInChildren<MonoBehaviour>(true);

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour == null)
                {
                    continue;
                }

                string scriptName = behaviour.GetType().Name.ToLower();

                if (scriptName.Contains("flicker"))
                {
                    flickerScripts.Add(behaviour);
                }
            }
        }

        return new List<Behaviour>(flickerScripts).ToArray();
    }
}

public class PickupSoundBlackoutRunner : MonoBehaviour
{
    public void StartSequence(
        AudioClip[] clips,
        float volume,
        bool playAs3DSound,
        bool waitForSoundToFinish,
        float manualBlackoutDelayFromPickup,
        float blackoutDelayAfterSound,
        Light[] lightsToTurnOff,
        Behaviour[] flickerScriptsToDisable,
        Behaviour[] componentsToDisableAfterSound,
        GameObject[] objectsToDeactivateAfterSound,
        Renderer[] renderersToDisableAfterSound,
        ParticleSystem[] particleSystemsToStopAfterSound,
        GameObject deathScreenObject,
        float deathScreenDelayAfterBlackout,
        bool pauseGameWhenDeathScreenAppears,
        bool showCursorOnDeathScreen
    )
    {
        List<AudioSource> audioSources = new List<AudioSource>();

        foreach (AudioClip clip in clips)
        {
            if (clip == null)
            {
                continue;
            }

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.spatialBlend = playAs3DSound ? 1f : 0f;
            audioSource.Play();

            audioSources.Add(audioSource);
        }

        StartCoroutine(BlackoutSequence(
            audioSources,
            waitForSoundToFinish,
            manualBlackoutDelayFromPickup,
            blackoutDelayAfterSound,
            lightsToTurnOff,
            flickerScriptsToDisable,
            componentsToDisableAfterSound,
            objectsToDeactivateAfterSound,
            renderersToDisableAfterSound,
            particleSystemsToStopAfterSound,
            deathScreenObject,
            deathScreenDelayAfterBlackout,
            pauseGameWhenDeathScreenAppears,
            showCursorOnDeathScreen
        ));
    }

    private IEnumerator BlackoutSequence(
        List<AudioSource> audioSources,
        bool waitForSoundToFinish,
        float manualBlackoutDelayFromPickup,
        float blackoutDelayAfterSound,
        Light[] lightsToTurnOff,
        Behaviour[] flickerScriptsToDisable,
        Behaviour[] componentsToDisableAfterSound,
        GameObject[] objectsToDeactivateAfterSound,
        Renderer[] renderersToDisableAfterSound,
        ParticleSystem[] particleSystemsToStopAfterSound,
        GameObject deathScreenObject,
        float deathScreenDelayAfterBlackout,
        bool pauseGameWhenDeathScreenAppears,
        bool showCursorOnDeathScreen
    )
    {
        if (waitForSoundToFinish)
        {
            bool anySoundStillPlaying = true;

            while (anySoundStillPlaying)
            {
                anySoundStillPlaying = false;

                foreach (AudioSource audioSource in audioSources)
                {
                    if (audioSource != null && audioSource.isPlaying)
                    {
                        anySoundStillPlaying = true;
                        break;
                    }
                }

                yield return null;
            }

            if (blackoutDelayAfterSound > 0f)
            {
                yield return new WaitForSeconds(blackoutDelayAfterSound);
            }
        }
        else
        {
            if (manualBlackoutDelayFromPickup > 0f)
            {
                yield return new WaitForSeconds(manualBlackoutDelayFromPickup);
            }
        }

        foreach (Behaviour flickerScript in flickerScriptsToDisable)
        {
            if (flickerScript != null)
            {
                flickerScript.enabled = false;
            }
        }

        foreach (Behaviour component in componentsToDisableAfterSound)
        {
            if (component != null)
            {
                component.enabled = false;
            }
        }

        foreach (Light light in lightsToTurnOff)
        {
            if (light != null)
            {
                light.enabled = false;
            }
        }

        foreach (Renderer rendererToDisable in renderersToDisableAfterSound)
        {
            if (rendererToDisable != null)
            {
                rendererToDisable.enabled = false;
            }
        }

        foreach (ParticleSystem particleSystem in particleSystemsToStopAfterSound)
        {
            if (particleSystem != null)
            {
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        foreach (GameObject objectToDeactivate in objectsToDeactivateAfterSound)
        {
            if (objectToDeactivate != null)
            {
                objectToDeactivate.SetActive(false);
            }
        }

        if (deathScreenObject != null)
        {
            yield return new WaitForSeconds(deathScreenDelayAfterBlackout);

            deathScreenObject.SetActive(true);

            if (showCursorOnDeathScreen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (pauseGameWhenDeathScreenAppears)
            {
                Time.timeScale = 0f;
            }
        }

        Destroy(gameObject, 0.2f);
    }
}