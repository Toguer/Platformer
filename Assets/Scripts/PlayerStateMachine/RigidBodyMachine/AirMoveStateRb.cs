using Unity.Mathematics;
using UnityEngine;

public class AirMoveStateRb : PlayerBaseStateRb
{
    public AirMoveStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.ShouldApplyHorizontalMovement = true;

        float currentHorizontalMag = new Vector2(Ctx.Velocity.x, Ctx.Velocity.z).magnitude;
        if (Ctx.isRunning)
        {
            Ctx.AirSpeed = Mathf.Max(currentHorizontalMag, Ctx.RunSpeed);
        }
        else
        {
            Ctx.AirSpeed = Mathf.Max(currentHorizontalMag, Ctx.WalkSpeed);
        }

        Debug.Log($"AirMove entered | AirSpeed={Ctx.AirSpeed:F2} | vel={currentHorizontalMag:F2}");
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        ApplyAirControl();
    }

    public override void ExitState()
    {
        Ctx.ShouldApplyHorizontalMovement = false;
        Debug.Log("AirMove exit");
    }

    public override void CheckSwitchStates()
    {
    }

    public override void InitializeSubState()
    {
    }

    private void ApplyAirControl()
    {
        Vector3 vel = Ctx.Velocity;
        Vector3 currentHorizontal = new Vector3(vel.x, 0f, vel.z);

        if (!Ctx.IsMovementPressed)
        {
            Ctx.TargetHorizontalVelocity = currentHorizontal;
            return;
        }

        Vector3 inputDir = Ctx.ConvertToCameraSpace(new Vector3(
            Ctx.CurrentMovementInput.x, 0f, Ctx.CurrentMovementInput.y)).normalized;
        float currentAlong = Vector3.Dot(currentHorizontal, inputDir);
        Vector3 lateral = currentHorizontal - inputDir * currentAlong;

        float desiredAlong =
            Ctx.AirSpeed * new Vector2(Ctx.CurrentMovementInput.x, Ctx.CurrentMovementInput.y).magnitude;

        float targetAlong;
        if (Mathf.Sign(currentAlong) == Mathf.Sign(desiredAlong) && Mathf.Abs(currentAlong) > Mathf.Abs(desiredAlong))
        {
            targetAlong = currentAlong;
        }
        else
        {
            targetAlong = desiredAlong;
        }

        Vector3 target = lateral + inputDir * targetAlong;
        target = Vector3.ClampMagnitude(target, Ctx.AirSpeed);
        Ctx.TargetHorizontalVelocity = target;
    }
}