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
        Vector3 velocity = Ctx.Velocity;
        Ctx.Rb.AddForce(Vector3.up * Ctx.InitialJumpVelocity, ForceMode.Impulse);

        Ctx.shouldApplyHorizontalMovement = true; // permite controlar en el aire
        Ctx.RequireNewJumpPress = true;

        InitializeSubState();
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.shouldApplyHorizontalMovement = false;
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
                    Debug.Log("Jetpack from Fall");
                    SwitchState(Factory.Jetpack());
                }
            }
            else
            {
                if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
                {
                    Ctx.CurrentMovementY = Ctx.JetpackForce;
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
            SetSubState(Factory.AirWalk());
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

        // Si suelta salto, aplica más gravedad durante la subida
        if (!Ctx.IsJumpPressed && velocity.y > 0)
        {
            gravity *= Ctx.FallMultiplier; // puedes usar un multiplicador extra si quieres más "snap"
        }

        velocity.y += gravity * Time.deltaTime;
        Ctx.Velocity = velocity;
    }
}