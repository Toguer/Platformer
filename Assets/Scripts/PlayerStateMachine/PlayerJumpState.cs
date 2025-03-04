using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerJumpState : PlayerBaseState, IRootState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.RequireNewJumpPress = true;
        Ctx.RemainingCoyoteTime = 0;
        InitializeSubState();
        Debug.Log("Entered Jump State");
        HandleJump();
    }

    public override void UpdateState()
    {
        Ctx.DashRemainingCooldown -= Time.deltaTime;
        if (Ctx.DashRemainingCooldown < 0)
        {
            Ctx.DashAlreadyUsed = false;
        }

        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        if (Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsEarthPressed && Ctx.Interactable is EarthWall)
        {
            SwitchState(Factory.Burrow());
        }
        else if (!Ctx.IsJumpPressed)
        {
            Debug.Log("Caida temprana");
            SwitchState(Factory.Fall());
        }
        else if (!Ctx.JetpackAlreadyUsed && Ctx.JetpackDuration > 0 && !Ctx.RequireNewJumpPress &&
                 Ctx.JetpackTrigger > 0.1f)
        {
            SwitchState(Factory.Jetpack());

            Debug.Log("JetPack From Jump");
        }
        else if (Ctx.CharacterController.isGrounded)
        {
            Debug.Log("Grounded From Jump");
            SwitchState(Factory.Grounded());
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SetSubState(Factory.Dash());
        }
        else if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
        else if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
    }

    void HandleJump()
    {
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocity;
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocity;
    }

    public void HandleGravity()
    {
        bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;
        if (isFalling)
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * Ctx.FallMultiplier * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * 0.5f, -20.0f);
        }
        else
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * Time.deltaTime);
            Ctx.AppliedMovementY = (previousYVelocity + Ctx.CurrentMovementY) * 0.5f;
        }
    }
}