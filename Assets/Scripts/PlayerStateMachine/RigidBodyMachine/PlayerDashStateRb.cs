using UnityEngine;

public class PlayerDashStateRb : PlayerBaseStateRb, IRootState
{
    private float _dashTimer;
    private Vector3 _dashDirection;

    public PlayerDashStateRb(RbPlayerStateMachine context, FactoryRigidBody factory) : base(context, factory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        _dashTimer = Ctx.DashDuration;

        // Dirección basada en input + cámara
        if (Ctx.CurrentMovementInput.sqrMagnitude > 0)
        {
            Vector3 camDir =
                Ctx.ConvertToCameraSpace(new Vector3(Ctx.CurrentMovementInput.x, 0f, Ctx.CurrentMovementInput.y));
            _dashDirection = new Vector3(camDir.x, 0f, camDir.z).normalized;
        }
        else
        {
            _dashDirection = new Vector3(Ctx.transform.forward.x, 0f, Ctx.transform.forward.z).normalized;
        }

        // Marcar dash como usado
        Ctx.DashAlreadyUsed = true;
        Ctx.DashRemainingCooldown = Ctx.DashCooldown;

        Ctx.ShouldApplyHorizontalMovement = true;
        Ctx.TargetHorizontalVelocity = _dashDirection * Ctx.DashSpeed;

        // Desactivar gravedad temporal (opcional)
        Ctx.Velocity = new Vector3(_dashDirection.x * Ctx.DashSpeed, Ctx.Velocity.y, _dashDirection.z * Ctx.DashSpeed);

        // Partículas / animaciones
        // Ctx.AnimatorRef.SetTrigger("dash");

        if (Ctx.DashParticles != null)
        {
            Ctx.DashParticles.Play();
        }

        Debug.Log("Inicial TargetHorizontalVelocity: " + Ctx.TargetHorizontalVelocity);
    }

    public override void UpdateState()
    {
        _dashTimer -= Time.deltaTime;
        Vector3 dashXZ = _dashDirection * Ctx.DashSpeed;
        Ctx.ShouldApplyHorizontalMovement = true; // por si acaso
        Ctx.TargetHorizontalVelocity = dashXZ;

        var v = Ctx.Velocity; // conserva la Y (suelo≈0, aire mantiene su vertical)
        v.x = dashXZ.x;
        v.z = dashXZ.z;
        Ctx.Velocity = v;

        CheckSwitchStates();
        Debug.Log("Update TargetHorizontalVelocity: " + Ctx.TargetHorizontalVelocity);
    }

    public override void ExitState()
    {
        Ctx.DashPressed = false;

        Vector3 baseDir;

        if (Ctx.IsGrounded && Ctx.CurrentMovementInput.sqrMagnitude <= 0f)
        {
            Ctx.TargetHorizontalVelocity = Vector3.zero;
            Ctx.ShouldApplyHorizontalMovement = false;

            Ctx.Velocity = new Vector3(0f, Ctx.Velocity.y, 0f);
            return;
        }

        if (Ctx.CurrentMovementInput.sqrMagnitude > 0f)
        {
            Vector3 moveDirCam = Ctx.ConvertToCameraSpace(
                new Vector3(Ctx.CurrentMovementInput.x, 0f, Ctx.CurrentMovementInput.y));
            baseDir = new Vector3(moveDirCam.x, 0f, moveDirCam.z);
        }
        else
        {
            Vector3 vXZ = Ctx.Velocity;
            vXZ.y = 0f;
            baseDir = vXZ.sqrMagnitude > 0.0001f ? vXZ : _dashDirection;
        }

        baseDir = baseDir.normalized;

        Vector3 walkXZ = baseDir * Ctx.WalkSpeed;

        Ctx.ShouldApplyHorizontalMovement = true;
        Ctx.TargetHorizontalVelocity = walkXZ;

        // Cortar el impulso horizontal
        Vector3 velocity = Ctx.Velocity;
        velocity.x = walkXZ.x;
        velocity.z = walkXZ.z;
        Ctx.Velocity = velocity;
        Debug.Log("Final TargetHorizontalVelocity: " + Ctx.TargetHorizontalVelocity);
    }

    public override void CheckSwitchStates()
    {
        if (_dashTimer <= 0f)
        {
            // Al terminar el dash, volvemos al root state
            if (Ctx.IsGrounded)
            {
                SwitchState(Factory.Grounded());
            }
            else
            {
                SwitchState(Factory.Fall());
            }
        }
    }

    public override void InitializeSubState()
    {
        // Dash no tiene subestados
    }

    public void HandleGravity()
    {
    }
}