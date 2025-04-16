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
        Vector3 inputDir = Ctx.CurrentMovementInput.magnitude > 0 ? Ctx.CurrentMovementInput.normalized : Vector2.up;
        Vector3 camDir = Ctx.ConvertToCameraSpace(new Vector3(inputDir.x, 0, inputDir.y));
        _dashDirection = camDir.normalized;

        // Marcar dash como usado
        Ctx.DashAlreadyUsed = true;
        Ctx.DashRemainingCooldown = Ctx.DashCooldown;

        // Desactivar gravedad temporal (opcional)
        Ctx.Velocity = new Vector3(_dashDirection.x * Ctx.DashSpeed, 0f, _dashDirection.z * Ctx.DashSpeed);

        // Partículas / animaciones
        // Ctx.AnimatorRef.SetTrigger("dash");

        if (Ctx.DashParticles != null)
        {
            Ctx.DashParticles.Play();
        }
    }

    public override void UpdateState()
    {
        _dashTimer -= Time.deltaTime;

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

    public override void ExitState()
    {
        Ctx.DashPressed = false;
        
        Ctx.DashPressed = false;

        // Cortar el impulso horizontal
        Vector3 velocity = Ctx.Velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        Ctx.Velocity = velocity;
    }

    public override void CheckSwitchStates()
    {
        // No cambiar durante dash
    }

    public override void InitializeSubState()
    {
        // Dash no tiene subestados
    }

    public void HandleGravity()
    {
        //No hace falta
    }
}