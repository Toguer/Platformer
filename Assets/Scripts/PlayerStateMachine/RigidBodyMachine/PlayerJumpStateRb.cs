using UnityEngine;

public class PlayerJumpStateRb : PlayerBaseStateRb, IRootState
{
    public PlayerJumpStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody factoryRigidBody)
        : base(currentContext, factoryRigidBody)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        // Aplicamos salto vertical
        Vector3 velocity = Ctx.Velocity;
        velocity.y = Ctx.InitialJumpVelocity;
        Ctx.Velocity = velocity;

        Ctx.RequireNewJumpPress = true; // evita saltar de nuevo hasta que suelte
        InitializeSubState();
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        // No hace falta nada por ahora
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SwitchState(Factory.Dash());
        }
        else if (Ctx.Velocity.y <= 0)
        {
            // Transición a caída cuando empieza a caer
            SwitchState(Factory.Fall());
        }else if (!Ctx.JetpackAlreadyUsed && Ctx.JetpackDuration > 0)
        {
            if (Ctx.IsGamepad)
            {
                if (Ctx.JetpackTrigger > 0.1f && !Ctx.RequireNewJumpPress)
                {
                    Ctx.CurrentMovementY = Ctx.JetpackForce;
                    Ctx.AppliedMovementY = Ctx.JetpackForce;
                    Debug.Log("Jetpack from Fall");
                    SwitchState(Factory.Jetpack());
                }
            }
            else
            {
                if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
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
        if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Idle());
        }
    }

    public void HandleGravity()
    {
        Vector3 velocity = Ctx.Velocity;
        float gravity = Physics.gravity.y;

        // Si suelta salto, aplicamos fallMultiplier para caer más rápido
        if (!Ctx.IsJumpPressed && velocity.y > 0)
        {
            gravity *= Ctx.FallMultiplier;
        }

        velocity.y += gravity * Time.deltaTime;
        Ctx.Velocity = velocity;
    }
}