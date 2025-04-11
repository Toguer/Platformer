using UnityEngine;

public class PlayerFallStateRb : PlayerBaseStateRb, IRootState
{

    public PlayerFallStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
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
        if (Ctx.IsInteractPressed && Ctx.IsNearSand())
        {
            SwitchState(Factory.Burrow());
        }
        else if (Ctx.IsJumpPressed && Ctx.RemainingCoyoteTime > 0)
        {
            Ctx.RemainingCoyoteTime = 0;
            Debug.Log("Usando el coyote Time!");
            SwitchState(Factory.Jump());
        }
        else if (Ctx.IsGrounded)
        {
            Ctx.DashAlreadyUsed = false;
            Debug.Log("Grounded from Fall");
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.JetpackAlreadyUsed && Ctx.JetpackDuration > 0)
        {
            if (Ctx.IsGamepad)
            {
                if (Ctx.JetpackTrigger > 0.1f)
                {
                    Ctx.CurrentMovementY = Ctx.JetpackForce;
                    Ctx.AppliedMovementY = Ctx.JetpackForce;
                    Debug.Log("Jetpack from Fall");
                    SwitchState(Factory.Jetpack());
                }
            }
            else
            {
                if (Ctx.IsJumpPressed)
                {
                    Ctx.CurrentMovementY = Ctx.JetpackForce;
                    Ctx.AppliedMovementY = Ctx.JetpackForce;
                    Debug.Log("Jetpack from Fall");
                    SwitchState(Factory.Jetpack());
                }
            }
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SetSubState(Factory.Dash());
        }
        else if (Ctx.IsMovementPressed && Ctx.CurrentMovementInput.magnitude > 0.5f)
        {
            SetSubState(Factory.Run());
        }
        else if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
        else if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
    }

    public void HandleGravity()
    {
        throw new System.NotImplementedException();
    }
}
