using UnityEngine;

public class PlayerJumpStateRb : PlayerBaseStateRb, IRootState
{
    public PlayerJumpStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody factoryRigidBody)
        : base(currentContext, factoryRigidBody)
    {
        IsRootState = true;
    }

    private float _jumpTimeElapsed;
    private bool _hasSwitched = false;

    public override void EnterState()
    {
        _jumpTimeElapsed = 0;

        Ctx.CanUseCoyote = false;
        Ctx.SuppressGravityFrame = true;
        Ctx.RemainingCoyoteTime = 0;
        Ctx.RequireNewJumpPress = true;
        Ctx.JustJumped = true;

        Vector3 v = Ctx.Velocity;
        v.y = Mathf.Max(0f, v.y);
        Ctx.Velocity = v;

        if (Ctx.HorizontalSpeed >= Ctx.SpeedThreshold)
        {
            Ctx.Rb.AddForce(Vector3.up * Ctx.InitialFastJumpVelocity, ForceMode.Impulse);
        }
        else
        {
            Ctx.Rb.AddForce(Vector3.up * Ctx.InitialJumpVelocity, ForceMode.Impulse);
        }

        Ctx.ShouldApplyHorizontalMovement = true;

        Ctx.TargetHorizontalVelocity = new Vector3(Ctx.Velocity.x, 0f, Ctx.Velocity.z);


        InitializeSubState();
        CurrentSubState?.EnterState();
    }

    public override void UpdateState()
    {
        _jumpTimeElapsed += Time.deltaTime;
        _hasSwitched = false;
        HandleGravity();
        Ctx.TryCornerCorrection();
        CheckSwitchStates();
        if (_hasSwitched) return;
    }


    public override void ExitState()
    {
        Ctx.ShouldApplyHorizontalMovement = false;
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.DashEnabled && Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SwitchState(Factory.Dash());
        }
        else if (_jumpTimeElapsed >= Ctx.MinJumpTime && Ctx.Velocity.y <= -0.1f && !Ctx.SuppressGravityFrame && !Ctx.FastJump)
        {
            Debug.Log("Jump -> Fall");
            // Transición a caída cuando empieza a caer
            _hasSwitched = true;
            SwitchState(Factory.Fall());
        }else if (_jumpTimeElapsed >= Ctx.MinFastJumpTime && Ctx.Velocity.y <= -0.1f && !Ctx.SuppressGravityFrame && Ctx.FastJump)
        {
            Debug.Log("Jump -> Fall");
            // Transición a caída cuando empieza a caer
            _hasSwitched = true;
            SwitchState(Factory.Fall());
        }
        else if (Ctx.JetpackEnabled && !Ctx.JetpackAlreadyUsed && Ctx.JetpackDuration > 0 && !Ctx.RequireNewJumpPress)
        {
            if (Ctx.IsGamepad)
            {
                if (Ctx.JetpackTrigger > 0.1f)
                {
                    Ctx.CurrentMovementY = Ctx.JetpackForce;
                    Debug.Log("Jetpack from Fall");
                    SwitchState(Factory.Jetpack());
                }
            }
            else
            {
                if (Ctx.JetpackTrigger > 0.1f)
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
            Debug.Log("Airmove set");
            SetSubState(Factory.AirMove());
        }
        else
        {
            SetSubState(Factory.Idle());
        }
    }

    public void HandleGravity()
    {
        if (Ctx.SuppressGravityFrame)
        {
            Ctx.SuppressGravityFrame = false;
            return; // NO aplicar gravedad el primer frame
        }

        Vector3 velocity = Ctx.Velocity;
        float gravity = Physics.gravity.y;

        // Si suelta salto, aplica más gravedad durante la subida
        if (!Ctx.IsJumpPressed && velocity.y > 0 && Ctx.HorizontalSpeed < Ctx.SpeedThreshold)
        {
            gravity *= Ctx.FallMultiplier;
        }
        else if (!Ctx.IsJumpPressed && velocity.y > 0 && Ctx.HorizontalSpeed > Ctx.SpeedThreshold)
        {
            gravity *= Ctx.FastJumpFallMultiplier;
        }

        velocity.y += gravity * Time.deltaTime;
        Ctx.Velocity = velocity;
    }
}