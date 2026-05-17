using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[Serializable, VolumeComponentMenu("Post-processing/Custom/BrutalFilmGrain")]
public sealed class BrutalFilmGrain : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Overall grain intensity.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0.6f, 0f, 2f);

    [Tooltip("Size of the grain pattern. Lower = finer grain, higher = chunkier grain.")]
    public ClampedFloatParameter grainScale = new ClampedFloatParameter(2.5f, 0.25f, 12f);

    [Tooltip("How fast the grain pattern moves.")]
    public ClampedFloatParameter speed = new ClampedFloatParameter(1.5f, 0f, 20f);

    [Tooltip("Boost grain in darker areas.")]
    public ClampedFloatParameter shadowBoost = new ClampedFloatParameter(1.75f, 0f, 4f);

    [Tooltip("Makes the grain harsher and less soft.")]
    public ClampedFloatParameter contrast = new ClampedFloatParameter(1.8f, 0.25f, 4f);

    [Tooltip("Use subtle colored grain instead of monochrome.")]
    public BoolParameter colored = new BoolParameter(false);

    private Material _material;

    public bool IsActive() => _material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint =>
        CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        Shader shader = Shader.Find("Hidden/Shader/BrutalFilmGrain");
        if (shader != null)
        {
            _material = new Material(shader);
        }
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (_material == null)
        {
            HDUtils.BlitCameraTexture(cmd, source, destination);
            return;
        }

        _material.SetTexture("_MainTex", source);
        _material.SetFloat("_Intensity", intensity.value);
        _material.SetFloat("_GrainScale", grainScale.value);
        _material.SetFloat("_Speed", speed.value);
        _material.SetFloat("_ShadowBoost", shadowBoost.value);
        _material.SetFloat("_Contrast", contrast.value);
        _material.SetFloat("_Colored", colored.value ? 1f : 0f);

        HDUtils.DrawFullScreen(cmd, _material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(_material);
    }
}