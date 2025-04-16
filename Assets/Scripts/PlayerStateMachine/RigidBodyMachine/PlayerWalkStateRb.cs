using UnityEngine;

public class PlayerWalkStateRb : PlayerBaseStateRb
{
    public PlayerWalkStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.shouldApplyHorizontalMovement = true;
        //Ctx.AnimatorRef.SetBool("isWalk", true);
        //Ctx.AudioPlayerRef.PlaySteps();
    }

    public override void UpdateState()
    {
        Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x;
        Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y;


        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.shouldApplyHorizontalMovement = false;
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
        else if (Ctx.IsMovementPressed && Ctx.CurrentMovementInput.magnitude > 0.5f)
        {
            //SwitchState(Factory.Run());
        }
    }

    public override void InitializeSubState()
    {
    }
}