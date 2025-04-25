using UnityEngine;

public class PlayerGroundedStateRb : PlayerBaseStateRb, IRootState
{
    public PlayerGroundedStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody factoryRigidBody) : base(
        currentContext, factoryRigidBody)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.shouldApplyHorizontalMovement = true;
        //Ctx.AnimatorRef.SetBool("isJump", false);
        InitializeSubState();

        Ctx.JetpackAlreadyUsed = false;
        Ctx.RequireNewJumpPress = false;

        // Cancelar velocidad vertical y frenar horizontalmente
        Vector3 velocity = Ctx.Velocity;
        velocity.y = 0f;

        // Freno inmediato (puedes cambiar a *= 0.1f si quieres un aterrizaje más suave)
        velocity.x *= 0.1f;
        velocity.z *= 0.1f;

        Ctx.Velocity = velocity;
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
        if (Ctx.RemainingJumpBufferTime > 0f && !Ctx.RequireNewJumpPress)
        {
            Ctx.RemainingJumpBufferTime = 0f;
            Debug.Log("Saltando desde Grounded");
            SwitchState(Factory.Jump());
        }
        else if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SwitchState(Factory.Dash());
        }
        else if (!Ctx.IsGrounded && Ctx.Velocity.y <= 0f)
        {
            Debug.Log("Cayendo desde Grounded");
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsInteractPressed && Ctx.IsNearSand())
        {
            Debug.Log("Entrando en BurrowState");
            SwitchState(Factory.Burrow());
        }
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
    }

    public void HandleGravity()
    {
    }
}