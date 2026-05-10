using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField]
    private float _castDistance = 5f;
    [SerializeField]
    private Vector3 _raycastOffset = new Vector3(0, 1f, 0);

    [SerializeField]
    private GameObject _InteractionUI;


    private void Update()
    {

        if (DoInteractionTest(out IInteractable interactable))
        {
            if (interactable.CanInteract())
            {
                _InteractionUI.SetActive(true);

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    interactable.Interact(this);
                }

            }
            else
            {
                _InteractionUI.SetActive(false);
            }
        }
        else
        {

            _InteractionUI.SetActive(false);
        }
    }

    private bool DoInteractionTest(out IInteractable interactable)
    {
        interactable = null;

        Ray ray = new Ray(transform.position + _raycastOffset, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _castDistance))
        {
            interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                return true;
            }
        }
       
        return false;
    }
}
