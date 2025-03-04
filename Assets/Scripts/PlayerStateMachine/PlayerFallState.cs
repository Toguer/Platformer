using UnityEngine;

public class PlayerFallState : PlayerBaseState, IRootState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
    }

    public void HandleGravity()
    {
        float previousYVelocity = Ctx.CurrentMovementY;
        Ctx.CurrentMovementY = Ctx.CurrentMovementY + Ctx.Gravity * Time.deltaTime;
        Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * .5f, -20.0f);
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsEarthPressed && Ctx.Interactable is EarthWall)
        {
            SwitchState(Factory.Burrow());
        }
        else if (Ctx.IsJumpPressed && Ctx.RemainingCoyoteTime > 0)
        {
            Ctx.RemainingCoyoteTime = 0;
            Debug.Log("Usando el coyote Time!");
            SwitchState(Factory.Jump());
        }
        else if (!Ctx.JetpackAlreadyUsed && Ctx.JetpackDuration > 0)
        {
            if (Ctx.IsGamepad)
            {
                if (Ctx.JetpackTrigger > 0.1f)
                {
                    Ctx.CurrentMovementY = Ctx.JetpackForce;
                    Ctx.AppliedMovementY = Ctx.JetpackForce;
                    Debug.Log("Jetpack from Fall");
                    SwitchState(Factory.Jetpack());
                }
            }
            else
            {
                if (Ctx.IsJumpPressed)
                {
                    Ctx.CurrentMovementY = Ctx.JetpackForce;
                    Ctx.AppliedMovementY = Ctx.JetpackForce;
                    Debug.Log("Jetpack from Fall");
                    SwitchState(Factory.Jetpack());
                }
            }
        }
        else if (Ctx.CharacterController.isGrounded)
        {
            Debug.Log("Grounded from Fall");
            SwitchState(Factory.Grounded());
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SetSubState(Factory.Dash());
        }
        else if (Ctx.IsMovementPressed && Ctx.CurrentMovementInput.magnitude > 0.5f)
        {
            SetSubState(Factory.Run());
        }
        else if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
        else if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
    }
}