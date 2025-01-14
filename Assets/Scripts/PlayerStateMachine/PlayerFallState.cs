using UnityEngine;

public class PlayerFallState : PlayerBaseState, IRootState
{
    private float _remainingCoyoteTime;
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        _remainingCoyoteTime = Ctx.CoyoteTime;
    }

    public override void UpdateState()
    {
        HandleGravity();
        _remainingCoyoteTime -= Time.deltaTime;
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
            SwitchState(Factory.Grounded());
        }else if (Ctx.IsJumpPressed && _remainingCoyoteTime > 0)
        {
            _remainingCoyoteTime = 0;
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