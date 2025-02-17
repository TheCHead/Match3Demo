using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    public Vector2 Selected => _selectAction.ReadValue<Vector2>();
    public bool IsFireTriggered => _fireAction.triggered;
    private PlayerInput _playerInput;
    private InputAction _selectAction;
    private InputAction _fireAction;

    private void Start() 
    {
        _playerInput = GetComponent<PlayerInput>();
        _selectAction = _playerInput.actions["Select"];
        _fireAction = _playerInput.actions["Fire"];
    }
}
