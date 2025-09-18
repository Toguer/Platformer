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
        Ctx.ShouldApplyHorizontalMovement = false;
        Ctx.TargetHorizontalVelocity = Vector3.zero;
        
        Vector3 v = Ctx.Velocity;
        Ctx.Velocity = new Vector3(0, v.y, 0);
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
        if (Ctx.DashEnabled && Ctx.DashPressed && !Ctx.DashAlreadyUsed)
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