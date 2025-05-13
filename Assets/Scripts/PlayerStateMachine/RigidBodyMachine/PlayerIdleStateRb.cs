using UnityEngine;

public class PlayerIdleStateRb : PlayerBaseStateRb
{
    public PlayerIdleStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody factoryRigidBody) : base(
        currentContext, factoryRigidBody)
    {
    }

    public override void EnterState()
    {
        //animaciones
        Ctx.shouldApplyHorizontalMovement = true;
        Ctx.TargetHorizontalVelocity = Vector3.zero;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            
            SwitchState(Factory.Dash());
        }
        if (Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Walk());
        }
    }

    public override void InitializeSubState()
    {
    }
}