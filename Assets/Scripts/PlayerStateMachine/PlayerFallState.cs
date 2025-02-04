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
        else if (!Ctx.JetpackAlreadyUsed && Ctx.IsJumpPressed)
        {
            Debug.Log("Jetpack from Fall");

            Ctx.CurrentMovementY = Mathf.Max(Ctx.CurrentMovementY, -2.0f);
            SwitchState(Factory.Jetpack());
        }
        else if (Ctx.IsJumpPressed && Ctx.RemainingCoyoteTime > 0)
        {
            Ctx.RemainingCoyoteTime = 0;
            Debug.Log("Usando el coyote Time!");
            SwitchState(Factory.Jump());
        }
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Run());
        }
    }
}