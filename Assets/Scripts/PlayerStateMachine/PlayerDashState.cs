using UnityEngine;

public class PlayerDashState : PlayerBaseState, IRootState
{
    private float _dashTimeRemaining;

    public PlayerDashState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.DashAlreadyUsed = true;
        Ctx.DashRemainingCooldown = Ctx.DashCooldown;
        _dashTimeRemaining = Ctx.DashDuration;
        Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x * Ctx.DashSpeed;
        Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y * Ctx.DashSpeed;
    }

    public override void UpdateState()
    {
        _dashTimeRemaining -= Time.deltaTime;
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.AppliedMovementX = 0;
        Ctx.AppliedMovementZ = 0;
    }

    public override void CheckSwitchStates()
    {
        
            if (Ctx.IsMovementPressed && Ctx.CurrentMovementInput.magnitude > 0.5f)
            {
                SwitchState(Factory.Run());
            }
            else if (Ctx.IsMovementPressed)
            {
                SwitchState(Factory.Walk());
            }
            else
            {
                SwitchState(Factory.Idle());
            }

    }

    public override void InitializeSubState()
    {
    }

    public void HandleGravity()
    {
    }
}