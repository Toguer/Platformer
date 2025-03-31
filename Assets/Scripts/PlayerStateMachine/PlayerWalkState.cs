using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.AnimatorRef.SetBool("isWalk", true);
        Ctx.AudioPlayerRef.PlaySteps();
    }

    public override void UpdateState()
    {
        Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x;
        Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y;

        if (Ctx.CurrentMovementInput.magnitude > 0.5f)
        {
            Ctx.AppliedMovementX *= Ctx.RunMultiplier;
            Ctx.AppliedMovementZ *= Ctx.RunMultiplier;
        }

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.AudioPlayerRef.StopSteps();
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SwitchState(Factory.Dash());
        }
        if (!Ctx.IsMovementPressed)
        {
            Ctx.AnimatorRef.SetBool("isWalk", false);
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && Ctx.CurrentMovementInput.magnitude > 0.5f)
        {
            SwitchState(Factory.Run());
        }
    }

    public override void InitializeSubState()
    {
    }
}