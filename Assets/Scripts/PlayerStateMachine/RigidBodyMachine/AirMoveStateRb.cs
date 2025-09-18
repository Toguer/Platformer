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
        Debug.Log("AirMove entered");
    }

    public override void UpdateState()
    {
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
}