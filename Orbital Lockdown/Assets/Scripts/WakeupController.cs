using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WakeupController : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera introCamera;
    [SerializeField] private Camera gameplayCamera;

    [Header("Intro Points")]
    [SerializeField] private Transform introStartPoint;
    [SerializeField] private Transform introEndPoint;

    [Header("Player Control")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Behaviour[] scriptsToDisableDuringIntro;

    [Header("Player Visibility During Intro")]
    [SerializeField] private Transform[] playerVisualRootsToHide;
    [SerializeField] private Renderer[] extraRenderersToHide;
    [SerializeField] private bool showPlayerModelAfterIntro = true;

    [Header("Intro Timing")]
    [SerializeField] private float waitBeforeShake = 0.3f;
    [SerializeField] private float shakeDuration = 1.5f;
    [SerializeField] private float waitAfterShake = 0.4f;
    [SerializeField] private float introMoveDuration = 5f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Impact Shake")]
    [SerializeField] private float shakePositionAmount = 0.06f;
    [SerializeField] private float shakeRotationAmount = 2.5f;
    [SerializeField] private float shakeFrequency = 35f;
    [SerializeField] private AnimationCurve shakeFadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("Impact Sound")]
    [SerializeField] private AudioSource impactAudioSource;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private float impactSoundVolume = 1f;

    [Header("Screen Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float startingBlackAlpha = 1f;
    [SerializeField] private float wakeVisibleAlpha = 0.15f;
    [SerializeField] private float wakeFadeInDuration = 1.5f;
    [SerializeField] private float cameraSwitchFadeOutDuration = 0.25f;
    [SerializeField] private float cameraSwitchFadeInDuration = 0.5f;
    [SerializeField] private bool disableFadeOverlayAfterIntro = true;

    [Header("Optional")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private bool logDebugMessages = true;

    [Header("Tutorial HUD")]
    [SerializeField] private TutorialHUDScript tutorialHUDScript;

    private AudioListener introAudioListener;
    private AudioListener gameplayAudioListener;

    private readonly List<Renderer> hiddenRenderers = new List<Renderer>();

    private void Start()
    {
        StartCoroutine(PlayWakeUpIntro());
    }

    private IEnumerator PlayWakeUpIntro()
    {
        if (logDebugMessages)
        {
            Debug.Log("Wakeup intro started.");
        }

        if (introCamera == null || gameplayCamera == null || introStartPoint == null || introEndPoint == null)
        {
            Debug.LogWarning("WakeupController is missing Intro Camera, Gameplay Camera, Intro Start Point, or Intro End Point.");
            yield break;
        }

        PrepareFadeOverlay();

        DisablePlayerControl();
        HidePlayerVisuals();

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(false);
        }

        if (tutorialHUDScript != null)
        {
            tutorialHUDScript.HideTutorialHUD();
        }

        introAudioListener = introCamera.GetComponent<AudioListener>();
        gameplayAudioListener = gameplayCamera.GetComponent<AudioListener>();

        if (introAudioListener != null)
        {
            introAudioListener.enabled = true;
        }

        if (gameplayAudioListener != null)
        {
            gameplayAudioListener.enabled = false;
        }

        gameplayCamera.gameObject.SetActive(false);
        introCamera.gameObject.SetActive(true);

        introCamera.transform.position = introStartPoint.position;
        introCamera.transform.rotation = introStartPoint.rotation;

        SetFadeAlpha(startingBlackAlpha);

        PlayImpactSound();

        if (waitBeforeShake > 0f)
        {
            yield return new WaitForSeconds(waitBeforeShake);
        }

        yield return StartCoroutine(PlayImpactShake());

        yield return StartCoroutine(FadeTo(wakeVisibleAlpha, wakeFadeInDuration));

        if (waitAfterShake > 0f)
        {
            yield return new WaitForSeconds(waitAfterShake);
        }

        yield return StartCoroutine(MoveIntroCameraToGameplayPosition());

        yield return StartCoroutine(FadeTo(1f, cameraSwitchFadeOutDuration));

        gameplayCamera.transform.position = introEndPoint.position;
        gameplayCamera.transform.rotation = introEndPoint.rotation;

        introCamera.gameObject.SetActive(false);
        gameplayCamera.gameObject.SetActive(true);

        if (introAudioListener != null)
        {
            introAudioListener.enabled = false;
        }

        if (gameplayAudioListener != null)
        {
            gameplayAudioListener.enabled = true;
        }

        if (showPlayerModelAfterIntro)
        {
            ShowPlayerVisuals();
        }

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }

        if (tutorialHUDScript != null)
        {
            tutorialHUDScript.ShowTutorialHUD();
        }

        EnablePlayerControl();

        yield return StartCoroutine(FadeTo(0f, cameraSwitchFadeInDuration));

        if (disableFadeOverlayAfterIntro && fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(false);
        }

        if (logDebugMessages)
        {
            Debug.Log("Wakeup intro finished.");
        }
    }

    private void PrepareFadeOverlay()
    {
        if (fadeCanvasGroup == null)
        {
            return;
        }

        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.alpha = startingBlackAlpha;
        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeCanvasGroup == null)
        {
            return;
        }

        fadeCanvasGroup.alpha = Mathf.Clamp01(alpha);
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (fadeCanvasGroup == null)
        {
            yield break;
        }

        fadeCanvasGroup.gameObject.SetActive(true);

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsedTime = 0f;

        if (duration <= 0f)
        {
            fadeCanvasGroup.alpha = Mathf.Clamp01(targetAlpha);
            yield break;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        fadeCanvasGroup.alpha = Mathf.Clamp01(targetAlpha);
    }

    private void PlayImpactSound()
    {
        if (impactAudioSource == null || impactSound == null)
        {
            return;
        }

        impactAudioSource.PlayOneShot(impactSound, impactSoundVolume);
    }

    private IEnumerator PlayImpactShake()
    {
        Vector3 basePosition = introStartPoint.position;
        Quaternion baseRotation = introStartPoint.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / shakeDuration);
            float shakeStrength = shakeFadeCurve.Evaluate(t);

            float noiseX = Mathf.PerlinNoise(Time.time * shakeFrequency, 0.1f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(0.2f, Time.time * shakeFrequency) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(Time.time * shakeFrequency, Time.time * shakeFrequency) - 0.5f;

            Vector3 positionOffset = new Vector3(noiseX, noiseY, noiseZ) * shakePositionAmount * shakeStrength;

            float rotationX = noiseY * shakeRotationAmount * shakeStrength;
            float rotationY = noiseX * shakeRotationAmount * shakeStrength;
            float rotationZ = noiseZ * shakeRotationAmount * shakeStrength;

            introCamera.transform.position = basePosition + positionOffset;
            introCamera.transform.rotation = baseRotation * Quaternion.Euler(rotationX, rotationY, rotationZ);

            yield return null;
        }

        introCamera.transform.position = basePosition;
        introCamera.transform.rotation = baseRotation;
    }

    private IEnumerator MoveIntroCameraToGameplayPosition()
    {
        float elapsedTime = 0f;

        if (logDebugMessages)
        {
            Debug.Log("Intro camera movement section reached.");
        }

        while (elapsedTime < introMoveDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / introMoveDuration);
            float curvedT = movementCurve.Evaluate(t);

            introCamera.transform.position = Vector3.Lerp(
                introStartPoint.position,
                introEndPoint.position,
                curvedT
            );

            introCamera.transform.rotation = Quaternion.Slerp(
                introStartPoint.rotation,
                introEndPoint.rotation,
                curvedT
            );

            yield return null;
        }

        introCamera.transform.position = introEndPoint.position;
        introCamera.transform.rotation = introEndPoint.rotation;

        if (logDebugMessages)
        {
            Debug.Log("Intro camera reached end point: " + introEndPoint.name);
        }
    }

    private void DisablePlayerControl()
    {
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        foreach (Behaviour script in scriptsToDisableDuringIntro)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void EnablePlayerControl()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
        }

        foreach (Behaviour script in scriptsToDisableDuringIntro)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HidePlayerVisuals()
    {
        hiddenRenderers.Clear();

        foreach (Transform visualRoot in playerVisualRootsToHide)
        {
            if (visualRoot == null)
            {
                continue;
            }

            Renderer[] renderers = visualRoot.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                if (renderer != null && renderer.enabled)
                {
                    renderer.enabled = false;
                    hiddenRenderers.Add(renderer);
                }
            }
        }

        foreach (Renderer renderer in extraRenderersToHide)
        {
            if (renderer != null && renderer.enabled)
            {
                renderer.enabled = false;
                hiddenRenderers.Add(renderer);
            }
        }

        if (logDebugMessages)
        {
            Debug.Log("Hidden player renderers: " + hiddenRenderers.Count);
        }
    }

    private void ShowPlayerVisuals()
    {
        foreach (Renderer renderer in hiddenRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = true;
            }
        }

        hiddenRenderers.Clear();

        if (logDebugMessages)
        {
            Debug.Log("Player visuals restored.");
        }
    }
}