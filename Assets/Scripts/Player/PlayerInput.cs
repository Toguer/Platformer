using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    //inputs del player, para acceder a ellos poner On + "nombre correcto del input" y coge el input, para usarlo haces un Get del input recogido
    private Vector3 playerInput;
    private bool _jump;

    public void OnMove(InputValue move)
    {
        Vector2 input = move.Get<Vector2>();
        playerInput = new Vector3(input.x, 0f, input.y);
    }

    public Vector3 GetMove()
    {
        return playerInput;
    }

    public void OnJumpPerformed(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            _jump = true;
            Debug.Log("Performed");
        }
        else if (callbackContext.canceled)
        {
            _jump = false;
            Debug.Log("Canceled");
        }
    }

    public bool GetJump()
    {
        return _jump;
    }
}