using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.AnimatorRef.SetBool("isRun", true);
    }

    public override void UpdateState()
    {
        Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x * Ctx.RunMultiplier;
        Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y * Ctx.RunMultiplier;
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.AnimatorRef.SetBool("isRun", false);
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SwitchState(Factory.Dash());
        }
        else if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && Ctx.CurrentMovementInput.magnitude < 0.5f)
        {
            SwitchState(Factory.Walk());
        }
    }

    public override void InitializeSubState()
    {
    }
}