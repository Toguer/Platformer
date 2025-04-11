using UnityEngine;

public class PlayerGroundedStateRb : PlayerBaseStateRb, IRootState
{
    public PlayerGroundedStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody factoryRigidBody) : base(currentContext, factoryRigidBody)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        //Ctx.AnimatorRef.SetBool("isJump", false);
        InitializeSubState();
        
        Ctx.JetpackAlreadyUsed = false;
    }

    public override void UpdateState()
    {
        
        
        Ctx.DashRemainingCooldown -= Time.deltaTime;
        if (Ctx.DashRemainingCooldown <= 0)
        {
            Ctx.DashAlreadyUsed = false;
            Ctx.DashRemainingCooldown = 0;
        }
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.RemainingCoyoteTime = Ctx.CoyoteTime;
    }

    public override void CheckSwitchStates()
    {
        // Si el jugador esta en el suelo y se pulsa saltar, cambia el estado al PlayerJumpState
        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
        {
            SwitchState(Factory.Jump());
        }
        else if (!Ctx.IsGrounded)
        {
            SwitchState(Factory.Fall());
        }else if (Ctx.IsInteractPressed && Ctx.IsNearSand())
        {
            Debug.Log("Entrando en BurrowState");
            SwitchState(Factory.Burrow());
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SetSubState(Factory.Dash());
        }
        else if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
    }

    public void HandleGravity()
    {
        
    }
}
