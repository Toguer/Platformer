using UnityEngine;

public class PlayerRunStateRb : PlayerBaseStateRb
{
    public PlayerRunStateRb(RbPlayerStateMachine ctx, FactoryRigidBody factory) : base(ctx, factory)
    {
    }

    public override void EnterState()
    {
        Ctx.shouldApplyHorizontalMovement = true;
        //Ctx.AnimatorRef.SetBool("isRun", true);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        ApplyHorizontalMovement();
    }

    public override void ExitState()
    {
        Ctx.shouldApplyHorizontalMovement = false;
        //Ctx.AnimatorRef.SetBool("isRun", false);
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        else if (!Ctx.isRunning && !Ctx.IsGamepad ||
                 Ctx.CurrentMovementInput.magnitude < Ctx.RunMagnitude && Ctx.IsGamepad)
        {
            SwitchState(Factory.Walk());
        }
    }

    private void ApplyHorizontalMovement()
    {
        Vector3 moveDir = Ctx
            .ConvertToCameraSpace(new Vector3(Ctx.CurrentMovementInput.x, 0f, Ctx.CurrentMovementInput.y)).normalized;

        float targetSpeed = Ctx.RunSpeed;
        Vector3 targetVelocity = moveDir * targetSpeed;

        Vector3 currentVel = Ctx.Velocity;
        Vector3 horizontalVelocity = Vector3.Lerp(
            new Vector3(currentVel.x, 0f, currentVel.z),
            targetVelocity,
            Ctx.Acceleration * Time.fixedDeltaTime
        );

        Ctx.Velocity = new Vector3(horizontalVelocity.x, currentVel.y, horizontalVelocity.z);
    }

    public override void InitializeSubState()
    {
    }
}