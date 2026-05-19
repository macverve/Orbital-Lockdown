using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _castDistance = 5f;
    [SerializeField] private Vector3 _raycastOffset = new Vector3(0, 1f, 0);
    [SerializeField] private GameObject _InteractionUI;

    [Header("Hover Visuals")]
    [SerializeField] private Material _highlightMaterial; // Drag your glowing/lit material here!

    private HashSet<string> _inventoryTokens = new HashSet<string>();

    private Renderer _lastActiveRenderer = null;
    private Material _originalMaterial = null;
    private IInteractable _lastHoveredInteractable = null;

    private void Update()
    {
        if (DoInteractionTest(out IInteractable interactable))
        {
            HandleHoverVisuals(interactable);

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
            ClearLastHover();
        }
    }

    private bool DoInteractionTest(out IInteractable interactable)
    {
        interactable = null;
        Ray ray = new Ray(transform.position + _raycastOffset, transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * _castDistance, Color.red);

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

    private void HandleHoverVisuals(IInteractable currentInteractable)
    {
        if (_lastHoveredInteractable != currentInteractable)
        {
            ClearLastHover();

            _lastHoveredInteractable = currentInteractable;

            if (_lastHoveredInteractable is MonoBehaviour mono)
            {
                // Find the renderer on this object or its children
                Renderer renderer = mono.GetComponentInChildren<Renderer>();
                if (renderer != null && _highlightMaterial != null)
                {
                    _lastActiveRenderer = renderer;
                    _originalMaterial = renderer.material; // Save the original look

                    // Swap to the highlight material
                    renderer.material = _highlightMaterial;
                }
            }
        }
    }

    private void ClearLastHover()
    {
        if (_lastActiveRenderer != null && _originalMaterial != null)
        {
            // Restore original material
            _lastActiveRenderer.material = _originalMaterial;
        }

        _lastActiveRenderer = null;
        _originalMaterial = null;
        _lastHoveredInteractable = null;
    }

    public void AddToken(string tokenName)
    {
        _inventoryTokens.Add(tokenName);
        Debug.Log($"Picked up key: {tokenName}");
    }

    public bool HasToken(string tokenName)
    {
        return _inventoryTokens.Contains(tokenName);
    }
}