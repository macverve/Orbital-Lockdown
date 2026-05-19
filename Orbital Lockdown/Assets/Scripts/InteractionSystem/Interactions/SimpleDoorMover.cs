using DG.Tweening;
using UnityEngine;

public class SimpleDoorRotator : MonoBehaviour
{
    [Header("Door Panel")]
    [SerializeField] private Transform _doorTarget;

    [Header("Rotation Settings")]
    [Tooltip("The local rotation values when the door is completely open.")]
    [SerializeField] private Vector3 _openRotationAngles;
    [SerializeField] private float _rotateDuration = 1.0f;

    private bool _isOpened = false;

    /// <summary>
    /// Call this function from your valve's ActivatorInteraction UnityEvent.
    /// </summary>
    public void OpenDoor()
    {
        if (_isOpened || _doorTarget == null) return;
        _isOpened = true;

        // Convert your inspector angles into a clean Quaternion to prevent weird warping/twisting
        Quaternion targetQuaternion = Quaternion.Euler(_openRotationAngles);

        // Smoothly rotates directly to the target orientation
        _doorTarget.DOLocalRotateQuaternion(targetQuaternion, _rotateDuration).SetEase(Ease.OutQuad);

        Debug.Log($"[Door System] {_doorTarget.name} rotated to open position.");
    }
}