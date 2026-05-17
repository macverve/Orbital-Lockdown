using DG.Tweening;
using UnityEngine;

public class ItemPickupInteraction : MonoBehaviour, IInteractable
{
    [SerializeField]
    private float _animationDuration = 0.5f;
    private bool _hasInteracted = false;

    public bool CanInteract()
    {
       if (_hasInteracted)
        {
          return false;
        }
       return true;
    }

    public bool Interact(Interactor interactor)
    {
        _hasInteracted = true;
        transform.DOMove(interactor.transform.position, _animationDuration).OnComplete(() => Destroy(gameObject));
        return true;
    }
}
