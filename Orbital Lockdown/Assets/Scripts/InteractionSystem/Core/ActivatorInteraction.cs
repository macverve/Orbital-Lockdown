using UnityEngine;
using UnityEngine.Events; 

public class ActivatorInteraction : MonoBehaviour, IInteractable
{
    [Header("Security Settings")]
    [SerializeField] private string _requiredKeyID = ""; // Leave empty "" for an unlocked button
    [SerializeField] private bool _canOnlyUseOnce = true;

    [Header("Interaction Events")]
    [SerializeField] private UnityEvent _onActivated;    // What happens when successfully used

    private bool _hasBeenActivated = false;

    public bool CanInteract()
    {
        // If it's a single-use button and already pressed, disable interaction
        if (_canOnlyUseOnce && _hasBeenActivated)
        {
            return false;
        }
        return true;
    }

    public bool Interact(Interactor interactor)
    {
       
        if (!string.IsNullOrEmpty(_requiredKeyID))
        {
            if (!interactor.HasToken(_requiredKeyID))
            {
                Debug.Log($"[Control Board] Access Denied. Requires: {_requiredKeyID}");
                return false;
            }
        }

        
        ExecuteActivation();
        return true;
    }

    private void ExecuteActivation()
    {
        _hasBeenActivated = true;
        Debug.Log($"[Control Board] {gameObject.name} Activated!");

        
        _onActivated?.Invoke();
    }
}