using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices.WindowsRuntime;

public class DoorInteraction : MonoBehaviour, IInteractable

{
    [SerializeField]
    private Vector3 _targetRotation = new Vector3(0,-100f, 0f);
    [SerializeField]
    private float _rotationSpeed = 3f;

    private bool _isOpen = false;

    public bool CanInteract()
    {
        return true;
    }

    public bool Interact(Interactor interactor)
    {
        if (_isOpen)
        {
            transform.DORotate(-_targetRotation, _rotationSpeed, RotateMode.WorldAxisAdd);
        }
        else
        {
            transform.DORotate(_targetRotation, _rotationSpeed, RotateMode.WorldAxisAdd);
        }

        _isOpen = !_isOpen;

        return true;

        }
    }
