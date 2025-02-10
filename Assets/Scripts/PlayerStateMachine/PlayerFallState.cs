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
        if (Ctx.CharacterController.isGrounded)
        {
            Debug.Log("Grounded from Fall");
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.IsJumpPressed && Ctx.RemainingCoyoteTime > 0)
        {
            Ctx.RemainingCoyoteTime = 0;
            Debug.Log("Usando el coyote Time!");
            SwitchState(Factory.Jump());
        }
        else if (!Ctx.JetpackAlreadyUsed && Ctx.IsJumpPressed && Ctx.JetpackDuration > 0)
        {
            Debug.Log("Jetpack from Fall");

            Ctx.CurrentMovementY = Ctx.JetpackForce;
            Ctx.AppliedMovementY = Ctx.JetpackForce;
            SwitchState(Factory.Jetpack());
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
        else if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
    }
}