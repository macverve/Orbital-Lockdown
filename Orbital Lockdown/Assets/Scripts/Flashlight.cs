using UnityEngine;
using UnityEngine.InputSystem; 

public class Flashlight : MonoBehaviour
{
    [Header("Drag the Flashlight object here")]
    [SerializeField] private Light _flashlightLight; 

    private bool _flashlightActive = false;

    void Start()
    {
       
        if (_flashlightLight != null)
        {
            _flashlightLight.enabled = false;
        }
    }

    void Update()
    {
       
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            _flashlightActive = !_flashlightActive;

            if (_flashlightLight != null)
            {
                _flashlightLight.enabled = _flashlightActive;
            }
        }
    }
}