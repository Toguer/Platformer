using UnityEngine;

public class PlayerWalkStateRb : PlayerBaseStateRb
{
    public PlayerWalkStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.ShouldApplyHorizontalMovement = true;
        //Ctx.AnimatorRef.SetBool("isWalk", true);
        //Ctx.AudioPlayerRef.PlaySteps();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        ApplyHorizontalMovement();
    }

    public override void ExitState()
    {
        Ctx.ShouldApplyHorizontalMovement = false;
        //Ctx.AudioPlayerRef.StopSteps();
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SwitchState(Factory.Dash());
        }
        else if (!Ctx.IsMovementPressed)
        {
            //Ctx.AnimatorRef.SetBool("isWalk", false);
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && Ctx.isRunning ||
                 Ctx.CurrentMovementInput.magnitude > Ctx.RunMagnitude && Ctx.IsGamepad)
        {
            SwitchState(Factory.Run());
        }
    }

    private void ApplyHorizontalMovement()
    {
        Vector3 moveDir = Ctx.ConvertToCameraSpace(new Vector3(
            Ctx.CurrentMovementInput.x, 0f, Ctx.CurrentMovementInput.y)).normalized;

        float targetSpeed = Ctx.WalkSpeed;
        Ctx.TargetHorizontalVelocity = moveDir * targetSpeed;
    }

    public override void InitializeSubState()
    {
    }
}