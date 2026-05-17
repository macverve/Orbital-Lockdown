using DG.Tweening;
using UnityEngine;

public class LockedDoorInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string _requiredKeyID = "GoldenKey"; // Must match the key's ID
    [SerializeField] private Transform _doorPivot;                // The part of the door that rotates/moves
    [SerializeField] private Vector3 _openRotation = new Vector3(0, 90f, 0);
    [SerializeField] private float _openDuration = 1.2f;

    private bool _isOpened = false;

    public bool CanInteract()
    {
        // The door can only be interacted with if it's currently closed
        return !_isOpened;
    }

    public bool Interact(Interactor interactor)
    {
        // Check if the player has the token required for this door
        if (interactor.HasToken(_requiredKeyID))
        {
            OpenDoor();
            return true;
        }

        Debug.Log("The door is locked. You need the " + _requiredKeyID);
        return false;
    }

    private void OpenDoor()
    {
        _isOpened = true;

        // Smoothly swing the door open using DOTween
        _doorPivot.DOLocalRotate(_openRotation, _openDuration).SetEase(Ease.OutBounce);

        Debug.Log("Door opened successfully!");
    }
}