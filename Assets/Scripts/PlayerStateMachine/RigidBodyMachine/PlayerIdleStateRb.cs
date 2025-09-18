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
        Debug.Log("IDLE");
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
        Debug.Log("Exit IDLE");
        Ctx.ShouldApplyHorizontalMovement = true;
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Walk());
        }
    }

    public override void InitializeSubState()
    {
    }
}