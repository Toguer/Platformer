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

        // --- MODO ESPECIAL: DASH DESDE WALL BURROW (usa forward 3D) ---
        if (Ctx.DashFromWallBurrow)
        {
            Vector3 _dir = Ctx.transform.forward;
            if (_dir.sqrMagnitude < 0.0001f)
            {
                _dir = Vector3.forward;
            }

            _dashDirection = _dir.normalized;

            Ctx.DashAlreadyUsed = true;
            Ctx.DashRemainingCooldown = Ctx.DashCooldown;

            Ctx.ShouldApplyHorizontalMovement = false;

            float _dashSpeed = Ctx.DashSpeed * Ctx.WallBurrowDashSpeedMultiplier;
            Ctx.Velocity = _dashDirection * _dashSpeed;

            if (Ctx.DashParticles != null)
            {
                Ctx.DashParticles.Play();
            }

            Debug.Log("Dash desde WallBurrow | dir=" + _dashDirection);
            return;
        }

        // --- MODO NORMAL (como lo tenías) ---
        if (Ctx.CurrentMovementInput.sqrMagnitude > 0)
        {
            Vector3 _camDir =
                Ctx.ConvertToCameraSpace(new Vector3(Ctx.CurrentMovementInput.x, 0f, Ctx.CurrentMovementInput.y));
            _dashDirection = new Vector3(_camDir.x, 0f, _camDir.z).normalized;
        }
        else
        {
            _dashDirection = new Vector3(Ctx.transform.forward.x, 0f, Ctx.transform.forward.z).normalized;
        }

        Ctx.DashAlreadyUsed = true;
        Ctx.DashRemainingCooldown = Ctx.DashCooldown;

        Ctx.ShouldApplyHorizontalMovement = true;
        Ctx.TargetHorizontalVelocity = _dashDirection * Ctx.DashSpeed;

        Ctx.Velocity = new Vector3(
            _dashDirection.x * Ctx.DashSpeed,
            Ctx.Velocity.y,
            _dashDirection.z * Ctx.DashSpeed
        );

        if (Ctx.DashParticles != null)
        {
            Ctx.DashParticles.Play();
        }

        Debug.Log("Inicial TargetHorizontalVelocity: " + Ctx.TargetHorizontalVelocity);
    }

    public override void UpdateState()
    {
        _dashTimer -= Time.deltaTime;

        if (Ctx.DashFromWallBurrow)
        {
            float _dashSpeed = Ctx.DashSpeed * Ctx.WallBurrowDashSpeedMultiplier;
            Vector3 _dashVel = _dashDirection * _dashSpeed;
            Ctx.Velocity = _dashVel;

            CheckSwitchStates();
            return;
        }

        Vector3 _dashXZ = _dashDirection * Ctx.DashSpeed;
        Ctx.ShouldApplyHorizontalMovement = true;
        Ctx.TargetHorizontalVelocity = _dashXZ;

        Vector3 _v = Ctx.Velocity;
        _v.x = _dashXZ.x;
        _v.z = _dashXZ.z;
        Ctx.Velocity = _v;

        CheckSwitchStates();
        Debug.Log("Update TargetHorizontalVelocity: " + Ctx.TargetHorizontalVelocity);
    }

    public override void ExitState()
    {
        Ctx.DashPressed = false;

        if (Ctx.DashFromWallBurrow)
        {
            // Dejamos que el siguiente estado decida la velocidad horizontal
            Ctx.DashFromWallBurrow = false;
            return;
        }

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