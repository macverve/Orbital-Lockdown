using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EscapePodValidator : MonoBehaviour, IInteractable
{
    [Header("Required Items for Launch")]
    [Tooltip("Type the exact item IDs needed to activate the pod (e.g., Wrench, Screwdriver, Keycard).")]
    [SerializeField] private List<string> _requiredItemIDs = new List<string>();

    [Header("Success Event")]
    [SerializeField] private UnityEvent _onPodActivated;

    private bool _isActivated = false;

    public bool CanInteract()
    {
        // Fixed: Removed the undefined _isOpened variable!
        // Now it just checks if the pod is already activated.
        return !_isActivated;
    }

    public bool Interact(Interactor interactor)
    {
        foreach (string itemID in _requiredItemIDs)
        {
            if (!interactor.HasToken(itemID))
            {
                Debug.Log($"[Escape Pod] Systems offline. Missing required asset: {itemID}");
                return false;
            }
        }

        ActivatePod();
        return true;
    }

    private void ActivatePod()
    {
        _isActivated = true;
        Debug.Log("[Escape Pod] All systems green! Launching sequence initiated!");

        _onPodActivated?.Invoke();
    }
}