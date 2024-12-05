using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActionAsset;

    private InputAction _moveAction;
    private InputAction _jumpAction;


    private void OnEnable()
    {
        _moveAction = _inputActionAsset.FindAction("Move");
        _jumpAction = _inputActionAsset.FindAction("Jump");
        
        _moveAction.Enable();
        _jumpAction.Enable();

        _jumpAction.performed += OnJumpPerformed;  
        _moveAction.performed += OnMovePerformed;
        
        _moveAction.canceled += OnMoveCanceled;
        _jumpAction.canceled += OnMoveCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();
        Debug.Log("Moving: " + movement);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        float jump = context.ReadValue<float>();
        Debug.Log("Jump: "+jump);
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Movement Canceled");
    }

    private void OnDisable()
    {
        _moveAction.performed -= OnMovePerformed;
        _moveAction.canceled -= OnMoveCanceled;
        _moveAction.Disable();
    }
}