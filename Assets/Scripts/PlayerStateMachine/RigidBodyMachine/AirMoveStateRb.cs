using UnityEngine;

public class AirMoveStateRb : PlayerBaseStateRb
{
    public AirMoveStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.shouldApplyHorizontalMovement = true;
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
        Ctx.shouldApplyHorizontalMovement = false;
    }

    public override void CheckSwitchStates()
    {
    }

    public override void InitializeSubState()
    {
    }
}