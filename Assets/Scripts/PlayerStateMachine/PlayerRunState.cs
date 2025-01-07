using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x * Ctx.RunMultiplier;
        Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.z * Ctx.RunMultiplier;
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SwitchState(Factory.Walk());
        }
    }

    public override void InitializeSubState()
    {
    }
}