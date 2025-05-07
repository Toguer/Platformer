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
        Debug.Log(
            $"[Enter Jump] velY: {Ctx.Velocity.y:F3} | isGrounded: {Ctx.IsGrounded} | suppressGravity: {Ctx.SuppressGravityFrame} | justJumped: {Ctx.JustJumped} | jumpSource: {Ctx.LastJumpSource}");
        Ctx.CanUseCoyote = false;
        Ctx.LogJumpDebug("Enter Jump");
        Ctx.SuppressGravityFrame = true;
        Ctx.RemainingCoyoteTime = 0;
        Vector3 v = Ctx.Velocity;
        v.y = Mathf.Max(0f, v.y); // ← Asegúrate de no tener velocidad descendente previa
        Ctx.Velocity = v;
        Ctx.Rb.AddForce(Vector3.up * Ctx.InitialJumpVelocity, ForceMode.Impulse);

        Ctx.JustJumped = true;
        Ctx.shouldApplyHorizontalMovement = true; // permite controlar en el aire
        Ctx.RequireNewJumpPress = true;

        InitializeSubState();
    }

    public override void UpdateState()
    {
        _jumpTimeElapsed += Time.deltaTime;
        _hasSwitched = false;
        HandleGravity();
        Ctx.TryCornerCorrection();
        CheckSwitchStates();
        if (_hasSwitched) return;
        Debug.Log("Velocity while jumping: " + Ctx.Velocity.y);
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
        else if (_jumpTimeElapsed >= Ctx.MinJumpTime && Ctx.Velocity.y <= -0.1f && !Ctx.SuppressGravityFrame)
        {
            Debug.Log("Jump -> Fall");
            // Transición a caída cuando empieza a caer
            _hasSwitched = true;
            SwitchState(Factory.Fall());
        }
        else if (!Ctx.JetpackAlreadyUsed && Ctx.JetpackDuration > 0 && !Ctx.RequireNewJumpPress)
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
            SetSubState(Factory.Walk());
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
        if (!Ctx.IsJumpPressed && velocity.y > 0)
        {
            gravity *= Ctx.FallMultiplier;
        }

        velocity.y += gravity * Time.deltaTime;
        Ctx.Velocity = velocity;
    }
}