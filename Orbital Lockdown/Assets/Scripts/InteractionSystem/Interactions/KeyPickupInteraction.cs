using DG.Tweening;
using UnityEngine;

public class KeyPickupInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string _keyID = "GoldenKey"; // Unique identifier for this key
    [SerializeField] private float _animationDuration = 0.5f;
    private bool _hasInteracted = false;

    public bool CanInteract()
    {
        return !_hasInteracted;
    }

    public bool Interact(Interactor interactor)
    {
        _hasInteracted = true;

        // Tell the player they now hold this key identifier
        interactor.AddToken(_keyID);

        PlaySoundWhenPickedUp pickupSound = GetComponent<PlaySoundWhenPickedUp>();

        if (pickupSound != null)
        {
            pickupSound.StartPickupSoundAndBlackout();
        }

        // Play pickup animation, then destroy the physical key object
        transform.DOMove(interactor.transform.position, _animationDuration)
            .OnComplete(() => Destroy(gameObject));

        return true;
    }
}