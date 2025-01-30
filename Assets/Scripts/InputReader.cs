using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    public Vector2 Selected => _selectAction.ReadValue<Vector2>();
    private PlayerInput _playerInput;
    private InputAction _selectAction;
    private InputAction _fireAction;

    public event Action FireEvent;

    private void Start() 
    {
        _playerInput = GetComponent<PlayerInput>();
        _selectAction = _playerInput.actions["Select"];
        _fireAction = _playerInput.actions["Fire"];

        _fireAction.performed += OnFireAction;
    }

    private void OnFireAction(InputAction.CallbackContext context)
    {
        FireEvent?.Invoke();
    }

    private void OnDestroy() 
    {
        _fireAction.performed -= OnFireAction;
    }
}
